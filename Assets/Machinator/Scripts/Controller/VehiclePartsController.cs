using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using RVP;

/// <summary>
/// 2026 01 27
///     Can add fucntion to just create and plus fill properties
///     Export settings for blender: 
///         Scale: 1.0
///         Apply settings: All local
///         Forward: -Z Forward
///         Up: Y up
///         Apply Unit
///         Use Space Transform
///         Apply Transform
/// </summary>



// Auto rotate 180 parent mainand set scale
// SPAWN CAB IRL
// SPAWN TURRET
// Put tires to real prefab


public class VehiclePartsController : MonoBehaviour
{
    [SerializeField] List<VehiclePart> vehicleParts = new List<VehiclePart>()
    {
        new VehiclePart("LP_CAB", "cab"),
        new VehiclePart("LP_BSK", "cargo")
    };
    [System.Serializable] class VehiclePart
    {
        public string lpName;
        public string modelName;
        public Transform lpTransform;
        public List<GameObject> prefabs = new List<GameObject>();

        public VehiclePart(string lp, string model)
        {
            lpName = lp;
            modelName = model;
        }
    }
    string chassisName = "chassis";

    // Suspension
    Dictionary<string, string> lpSuspensionWheels = new Dictionary<string, string>()
    {
        { "LP_SSP", "suspension" },
        { "LP_WHL", "wheel" }
    };
    
    [Header("Weapons Targeting")]
    public float distance = 100.0f;
    public LayerMask layerMask = 1;



    string chassis = "chassis";
    public Vector3 rotationOffset = new Vector3(0, 180.0f, 0);

    /// <summary>
    /// To do: sort by cab index name for scout 03
    /// </summary>
    public void SetCabAndCargo()
    {
        // Reset list back (don't forget)
        Undo.RecordObject(this, "Set Cab and Cargo");

        
        // Fix models
        List<GameObject> childList = new List<GameObject>();
        foreach (Transform child in transform)
        {
            childList.Add(child.gameObject);


           
        }

        // Chagne to transform
        childList.Sort((a, b) => a.name.CompareTo(b.name));


        foreach (GameObject child in childList)
        {
            GameObject parent = new GameObject("new " + child.name);
            Undo.RegisterCreatedObjectUndo(parent, "Set Cab Cargo");
            Undo.SetTransformParent(parent.transform, transform, "Set Cab Cargo");
            Undo.SetTransformParent(child.transform, parent.transform, "Set Cab Cargo");

            child.transform.position = new Vector3();
            child.transform.rotation = Quaternion.Euler(rotationOffset);
        }
        // sort here


       

        //return;

        Transform[] allTransformsSS = transform.GetComponentsInChildren<Transform>(true);
        // Find "LP_"
        foreach (Transform transformA in allTransformsSS)
        {
            foreach (VehiclePart vehiclePart in vehicleParts)
            {
                if (transformA.name.Contains(vehiclePart.lpName))
                {
                    GameObject parent = new GameObject("new " + vehiclePart.lpName);
                    Undo.RegisterCreatedObjectUndo(parent, "Set Cab Cargo");
                    Undo.SetTransformParent(parent.transform, transform, "Set Cab Cargo");
                    //Undo.SetTransformParent(transformA, parent.transform, "Set Cab Cargo");

                    parent.transform.position = transformA.transform.position;

                    // Find "cab" "cargo" models
                    foreach (Transform transformB in allTransformsSS)
                    {
                        if (transformB.name.Contains("new " + vehiclePart.modelName))
                        {
                            //Debug.Log(transformB.name);
                            Undo.RegisterFullObjectHierarchyUndo(transformB, "Set Cab And Cargo");


                            // UNDO BUG
                            // Put where it belongs
                            transformB.SetParent(parent.transform);
                            transformB.localPosition = Vector3.zero;

                            // sort by index
                            // AD script to  childrens 


                        }


                        // Filter cab inside a cab LOL
                            /* if (transformB.name.Contains(cabCargoLP.modelName) &&
                                 transformB.parent.GetComponent<VehiclePartsController>())
                             {
                                 Undo.RegisterFullObjectHierarchyUndo(transformB, "Set Cab And Cargo");

                                 // Put where it belongs
                                 transformB.SetParent(transformA);
                                 transformB.localPosition = Vector3.zero;
                                 //transformB.localRotation = Quaternion.identity;

                                 // Add script for scripts
                                 if (!transformB.gameObject.GetComponent<CabCargo>())
                                 {
                                     CabCargo CabCargo = transformB.gameObject.AddComponent<CabCargo>();
                                     CabCargo.SetCabAndCargo();
                                 }
                             }*/


                    }
                }
            }
        }

        // LP and sort nahui



        return;




        // Fix models
     


        //if chassis unknown
        List<Transform> knownLP = new List<Transform>();
        Transform[] allTransformsS = transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform transformA in allTransformsS)
        {
            foreach (VehiclePart vehiclePart in vehicleParts)
            {
                // Find LP to put them to
                if (transformA.name.Contains(vehiclePart.lpName))
                {
                    GameObject parent = new GameObject("new " + vehiclePart.lpName);
                    Undo.RegisterCreatedObjectUndo(parent, "Set Cab Cargo");
                    Undo.SetTransformParent(parent.transform, transform, "Set Cab Cargo");
                    Undo.SetTransformParent(transformA, parent.transform, "Set Cab Cargo");

                    parent.transform.position = transformA.transform.position;

                    knownLP.Add(parent.transform);
                }
            }
        }

        // put to list reference






            return;


        Transform[] allTransforms = transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform transformA in allTransforms)
        {
            // Chasiss
            if (transformA.name.Contains(chassis))
            {
                GameObject parent = new GameObject("new " + chassis);
                Undo.RegisterCreatedObjectUndo(parent, "Set Cab Cargo");
                Undo.SetTransformParent(parent.transform, transform, "Set Cab Cargo");
                Undo.SetTransformParent(transformA, parent.transform, "Set Cab Cargo");
            }

            // one to many
            foreach (VehiclePart vehiclePart in vehicleParts)
            {
                // Find LP to put them to
                if (transformA.name.Contains(vehiclePart.lpName))
                {
                    GameObject parent = new GameObject("new " + vehiclePart.lpName);
                    Undo.RegisterCreatedObjectUndo(parent, "Set Cab Cargo");
                    Undo.SetTransformParent(parent.transform, transform, "Set Cab Cargo");
                    Undo.SetTransformParent(transformA, parent.transform, "Set Cab Cargo");
                }

                // Find "LP_"
                if (transformA.name.Contains(vehiclePart.modelName))
                {
                    GameObject parent = new GameObject("new " + vehiclePart.modelName);
                    Undo.RegisterCreatedObjectUndo(parent, "Set Cab Cargo");
                    Undo.SetTransformParent(parent.transform, transform, "Set Cab Cargo");
                    Undo.SetTransformParent(transformA, parent.transform, "Set Cab Cargo");

                    // Now add scritp to 

                }
            }
        }

        // Put cabs to LP



    }


    /*
    public void SetCabAndCargo()
    {
        // Reset list back (don't forget)
        Undo.RecordObject(this, "Set Cab and Cargo");

        Transform[] allTransforms = transform.GetComponentsInChildren<Transform>(true);
        // Find "LP_"
        foreach (Transform transformA in allTransforms)
        {
            foreach (VehiclePart cabCargoLP in vehicleParts)
            {
                if (transformA.name.Contains(cabCargoLP.lpName))
                {
                    // Set spawn point for cab/cargo prefab here
                    cabCargoLP.lpTransform = transformA;

                    // Find "cab" "cargo" models
                    foreach (Transform transformB in allTransforms)
                    {
                        // Filter cab inside a cab LOL
                        if (transformB.name.Contains(cabCargoLP.modelName) &&
                            transformB.parent.GetComponent<VehiclePartsController>())
                        {
                            Undo.RegisterFullObjectHierarchyUndo(transformB, "Set Cab And Cargo");

                            // Put where it belongs
                            transformB.SetParent(transformA);
                            transformB.localPosition = Vector3.zero;
                            //transformB.localRotation = Quaternion.identity;

                            // Add script for scripts
                            if (!transformB.gameObject.GetComponent<CabCargo>())
                            {
                                CabCargo CabCargo = transformB.gameObject.AddComponent<CabCargo>();
                                CabCargo.SetCabAndCargo();
                            }
                        }
                    }
                }
            }
            // Reset chassis
            if (transformA.name.Contains(chassisName))
            {
                Undo.RecordObject(transformA, "Set Cab And Cargo");
                transformA.localPosition = Vector3.zero;
            }
        }
    }
    */
    /// <summary>
    /// To do: // Index out of bounds
    /// </summary>
    /// <param name="cabIndex"></param>
    /// <param name="cargoIndex"></param>
    public void ShowCabByIndex(int index)
    {
        if (vehicleParts[0].lpTransform == null)
        {
            Debug.LogError("No Cab detected!");

            return;
        }

        if (index < 0 || vehicleParts[0].lpTransform.childCount <= index)
        {
            Debug.LogError("Cab index is out of range");

            return;
        }

        DisableCabAndCargo(0);

        vehicleParts[0].lpTransform.GetChild(index).gameObject.SetActive(true);
    }

    public void ShowCargoByIndex(int index)
    {
        if (vehicleParts[1].lpTransform == null)
        {
            Debug.LogError("No Cargo detected!");

            return;
        }

        if (index < 0 || vehicleParts[1].lpTransform.childCount <= index)
        {
            Debug.LogError("Cargo index is out of range");

            return;
        }

        DisableCabAndCargo(1);

        vehicleParts[1].lpTransform.GetChild(index).gameObject.SetActive(true);
    }

    void DisableCabAndCargo(int index)
    {
        // No cargo case
        foreach (Transform transform in vehicleParts[index].lpTransform)
        {
            Undo.RegisterCompleteObjectUndo(transform, "Show Cab And Cargo By Index");
            transform.gameObject.SetActive(false);
        }
    }

    public void ShowCabBreakablesByIndex(int index)
    {
        if (vehicleParts[0].lpTransform == null)
        {
            Debug.LogError("No Cab detected!");

            return;
        }

        foreach (Transform transform in vehicleParts[0].lpTransform)
        {
            transform.gameObject.GetComponent<CabCargo>().ShowBreakablesByIndex(index);
        }
    }

    public void ShowCargoBreakablesByIndex(int index)
    {
        if (vehicleParts[1].lpTransform == null)
        {
            Debug.LogError("No Cargo detected!");

            return;
        }

        foreach (Transform transform in vehicleParts[1].lpTransform)
        {
            transform.gameObject.GetComponent<CabCargo>().ShowBreakablesByIndex(index);
        }
    }

    public void SetSuspensionAndWheels()
    {
        Transform[] transforms = transform.GetComponentsInChildren<Transform>(true);
        List<Transform> usedTransforms = new List<Transform>();
        foreach (var pair in lpSuspensionWheels)
        {
            // Find "LP_"
            foreach (Transform parent in transforms)
            {
                if (parent.name.Contains(pair.Key))
                {
                    // Filter "LP_01" and "LP_02"
                    string index = Regex.Match(parent.name, @"\d+").Value;
                    foreach (Transform child in transforms)
                    {
                        if (child.name.Contains(pair.Value) &&
                            child.name.Contains(index) &&
                            !usedTransforms.Contains(child))
                        {
                            Undo.SetTransformParent(child, parent, "Move Part");
                            child.localPosition = Vector3.zero;
                            child.localRotation = Quaternion.identity;

                            usedTransforms.Add(child);

                            // LP has one element
                            break;
                        }
                    }
                }
            }
        }
    }

    public void HideSuspensionAndWheels()
    {
        Transform[] transforms = transform.GetComponentsInChildren<Transform>(true);
        foreach (var pair in lpSuspensionWheels)
        {
            // Find "LP_"
            foreach (Transform transform in transforms)
            {
                if (transform.name.Contains(pair.Key))
                {
                    if (transform.childCount <= 0)
                    {
                        Debug.LogError($"No {pair.Value} Found");

                        break;
                    }
                    Transform child = transform.GetChild(0);

                    Undo.RegisterCompleteObjectUndo(child, "Hide Suspension And Wheels");
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;


    }
    
    void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, distance, layerMask))
        {
            targetPoint = hit.point;
        }
        else
        {
            // Prevent loosing target when shooting at long distance / or sky
            targetPoint = ray.GetPoint(distance);
        }
        Debug.DrawRay(ray.origin, ray.direction * 6000, Color.yellow);

        foreach (VehiclePart vehiclePart in vehicleParts)
        {
            if (vehiclePart.lpTransform != null)
            {
                foreach (var weaponSlot in vehiclePart.lpTransform.GetChild(0).GetComponent<CabCargo>().weaponSlots)
                {
                    weaponSlot.LookAtTarget(targetPoint);
                }
            }
        }
    }
}