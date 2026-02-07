using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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



// SPAWN CAB IRL
// SPAWN TURRET
// Put tires to real prefab
// Undo actions


public class VehiclePartsController : MonoBehaviour
{
    [Header("Prefabs")]
    public List<GameObject> cabPrefabs = new List<GameObject>();
    public List<GameObject> cargoPrefabs = new List<GameObject>();

    [Header("Load Points")]
    // Cab Cargo
    [SerializeField] List<CabCargoLP> CabCargoLPs = new List<CabCargoLP>()
    {
        new CabCargoLP("LP_CAB", "cab", null),
        new CabCargoLP("LP_BSK", "cargo", null)
    };
    [System.Serializable] class CabCargoLP
    {
        public string lpName;
        public string modelName;
        public Transform lpTransform;

        public CabCargoLP(string lp, string model, Transform transform)
        {
            lpName = lp;
            modelName = model;
            lpTransform = transform;
        }
    }
    // Suspension
    Dictionary<string, string> lpSuspensionWheels = new Dictionary<string, string>()
    {
        { "LP_SSP", "suspension" },
        { "LP_WHL", "wheel" }
    };
    
    [Header("Weapons Targeting")]
    public float distance = 100.0f;
    public LayerMask layerMask;

    
    
    /// <summary>
    /// To do: sort by cab index name for scout 03
    /// </summary>
    public void SetCabAndCargo()
    {
        // Reset list back (don't forget)
        Undo.RecordObject(this, "Set Cab and Cargo");

        Transform[] allTransforms = transform.GetComponentsInChildren<Transform>(true);
        foreach (CabCargoLP cabCargoLP in CabCargoLPs)
        {
            // Find "LP_"
            foreach (Transform transformA in allTransforms)
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
                            Undo.RegisterFullObjectHierarchyUndo(transformB, "Set Cab and Cargo");

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




















    public void SetTurretSlots()
    {

    }



    public void SpawnCab(int index)
    {
        GameObject cab = Instantiate(cabPrefabs[index], CabCargoLPs[0].lpTransform);

        // Have list of turret positinos
        //cab.GetComponent<CabCargoController>().turretLPs[0].position = Vector3.zero;
    }

    public void SpawnWeapons()
    {
        // To current cab

        // Call weapon from list or something global idk yet






    }

    public void SpawnCargo()
    {

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
        
        /// LOOK AT CAB
        //foreach (var slot in turretSlots)
        //{
        // /   slot.LookAtTarget(targetPoint);
        //}
    }





    















    /// <summary>
    /// To do: // Index out of bounds
    /// </summary>
    /// <param name="cabIndex"></param>
    /// <param name="cargoIndex"></param>
    public void ShowCabCargoByIndex(int cabIndex, int cargoIndex)
    {
        if (CabCargoLPs[0].lpTransform == null)
        {
            Debug.Log("No cab detected");

            return;
        }

        foreach (CabCargoLP cabCargoLP in CabCargoLPs)
        {
            if (cabCargoLP.lpTransform != null)
            {
                foreach (Transform transform in cabCargoLP.lpTransform)
                {
                    Undo.RegisterCompleteObjectUndo(transform, "Set Cab and Cargo");
                    transform.gameObject.SetActive(false);
                }
            }
        }
        CabCargoLPs[0].lpTransform.GetChild(cabIndex).gameObject.SetActive(true);

        if (CabCargoLPs[1].lpTransform == null)
        {
            Debug.Log("No cargo detected");

            return;
        }

        CabCargoLPs[1].lpTransform.GetChild(cargoIndex).gameObject.SetActive(true);
    }






    public void ShowCabCargoBreakablesByIndex(int breakableIndex)
    {
        foreach (CabCargoLP cabCargoLP in CabCargoLPs)
        {
            if (cabCargoLP.lpTransform != null)
            {
                foreach (Transform transform in cabCargoLP.lpTransform)
                {
                    transform.gameObject.GetComponent<CabCargo>().ShowBreakablesByIndex(breakableIndex);
                }
            }
        }
    }

    public void HideSuspensionAndWheels()
    {
        //  null cehck

        Transform[] transforms = transform.GetComponentsInChildren<Transform>(true);
        foreach (var pair in lpSuspensionWheels)
        {
            // Find "LP_"
            foreach (Transform transform in transforms)
            {
                if (transform.name.Contains(pair.Key))
                {
                    Transform child = transform.GetChild(0);
                    Undo.RegisterCompleteObjectUndo(child, "Set Cab and Cargo");

                    child.gameObject.SetActive(false);
                }
            }
        }


    }
}