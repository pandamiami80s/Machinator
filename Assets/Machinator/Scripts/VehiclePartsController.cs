using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UIElements;
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
// Put tires to real prefab
// Undo actions


public class VehiclePartsController : MonoBehaviour
{
    [System.Serializable] class CabCargoLP
    {
        public string lpName;
        public string modelName;
        public Transform lpTransform;

        // Constructor to make adding items easier
        public CabCargoLP(string lp, string model, Transform transform)
        {
            lpName = lp;
            modelName = model;
            lpTransform = transform;
        }
    }

    [SerializeField] List<CabCargoLP> CabCargoLPs = new List<CabCargoLP>()
    {
        new CabCargoLP("LP_CAB", "cab", null),
        new CabCargoLP("LP_BSK", "cargo", null)
    };

    Dictionary<string, string> lpSuspensionWheels = new Dictionary<string, string>()
    {
        { "LP_SSP", "suspension" },
        { "LP_WHL", "wheel" }
    };



    /// <summary>
    /// To do: sort by cab index name
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
                            if (!transformB.gameObject.GetComponent<CabCargoController>())
                            {
                                CabCargoController cabCargoController = transformB.gameObject.AddComponent<CabCargoController>();
                                cabCargoController.SetBreakables();
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
                    transform.gameObject.GetComponent<CabCargoController>().ShowBreakablesByIndex(breakableIndex);
                }
            }
        }
    }

    /// <summary>
    /// No undo here
    /// </summary>
    public void HideSuspensionAndWheels()
    {
        Transform[] transforms = transform.GetComponentsInChildren<Transform>(true);
        foreach (var pair in lpSuspensionWheels)
        {
            // Find "LP_"
            foreach (Transform parent in transforms)
            {
                if (parent.name.Contains(pair.Key))
                {
                    parent.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }
}