using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 2026 02 19
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
        // Spawn cab or cargo at
        public Transform lpTransform;
        public List<GameObject> prefabs = new List<GameObject>();

        public VehiclePart(string lp, string model)
        {
            lpName = lp;
            modelName = model;
        }
    }

    public void SetVehicleParts()
    {
        Utils.FixModels(transform);
        string[] allLPs = vehicleParts.Select(p => p.lpName).ToArray();
        Utils.ExtractLoadPoints(transform, allLPs);

        // Set CabCargoController
        List<Transform> allTransforms = Utils.GetChildTransforms(transform);
        foreach (VehiclePart vehiclePart in vehicleParts)
        {
            // LP
            foreach (Transform childA in allTransforms)
            {
                if (childA.name.Contains(vehiclePart.lpName))
                {
                    // Model
                    foreach (Transform childB in allTransforms)
                    {
                        if (childB.name.Contains(vehiclePart.modelName))
                        {
                            Undo.SetTransformParent(childB, childA, "Set Vehicle Parts");
                            childB.localPosition = Vector3.zero;
                            vehiclePart.lpTransform = childA;
                            // Script
                            if (!childB.GetComponent<CabCargoController>())
                            {
                                CabCargoController cabCargoController = Undo.AddComponent<CabCargoController>(childB.gameObject);
                                cabCargoController.SetCabCargo();
                            }
                        }
                    }
                }
            }
        }
    }

    public void ShowPartsByIndex(int partIndex, int childIndex)
    {
        if (childIndex < 0)
        {
            return;
        }

        if (vehicleParts[partIndex].lpTransform == null)
        {
            return;
        }

        foreach (Transform child in vehicleParts[partIndex].lpTransform)
        {
            Undo.RecordObject(child.gameObject, "Hide parts");
            child.gameObject.SetActive(false);
        }

        GameObject target = vehicleParts[partIndex].lpTransform.GetChild(childIndex).gameObject;
        target.SetActive(true);
    }

    public void ShowAllParts(int partIndex)
    {
        if (vehicleParts[partIndex].lpTransform == null)
        {
            return;
        }

        foreach (Transform child in vehicleParts[partIndex].lpTransform)
        {
            Undo.RecordObject(child.gameObject, "Show Parts");
            child.gameObject.SetActive(true);
        }
    }

    public void ShowBreakablesByIndex(int partIndex, int index)
    {
        if (vehicleParts[partIndex].lpTransform == null)
        {
            return;
        }

        foreach (Transform child in vehicleParts[partIndex].lpTransform)
        {
            child.gameObject.GetComponent<CabCargoController>().ShowBreakablesByIndex(index);
        }
    }

    public void ShowAllBreakables(int partIndex)
    {
        if (vehicleParts[partIndex].lpTransform == null)
        {
            return;
        }

        foreach (Transform child in vehicleParts[partIndex].lpTransform)
        {
            child.gameObject.GetComponent<CabCargoController>().ShowAllBreakables();
        }
    }
}

[CustomEditor(typeof(VehiclePartsController))]
public class VehiclePartsEditor : Editor
{
    // Not to run in OnInspectorGUI every time
    VehiclePartsController VehiclePartsController => (VehiclePartsController)target;
    int cabIndex;
    int cabBreakableIndex;
    int cargoIndex;
    int cargoBreakableIndex;



    public override void OnInspectorGUI()
    {
        // Do not replace the entire Inspector view
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUILayout.Label("Editor");
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Set Vehicle Parts"))
        {
            VehiclePartsController.SetVehicleParts();
            EditorUtility.SetDirty(VehiclePartsController);
        }
        
        GUILayout.Label("Cab");
        GUI.backgroundColor = Color.white;
        cabIndex = EditorGUILayout.IntField("Index", cabIndex);
        GUI.backgroundColor = Color.darkRed;
        if (GUILayout.Button("Show by Index"))
        {
            VehiclePartsController.ShowPartsByIndex(0, cabIndex);
            EditorUtility.SetDirty(VehiclePartsController);
        }

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show All"))
        {
            VehiclePartsController.ShowAllParts(0);
            EditorUtility.SetDirty(VehiclePartsController);
        }

        GUI.backgroundColor = Color.white;
        cabBreakableIndex = EditorGUILayout.IntField("Breakable Index", cabBreakableIndex);
        GUI.backgroundColor = Color.darkRed;
        if (GUILayout.Button("Show Breakables by Index"))
        {
            VehiclePartsController.ShowBreakablesByIndex(0, cabBreakableIndex);
            EditorUtility.SetDirty(VehiclePartsController);
        }

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show All Breakables"))
        {
            VehiclePartsController.ShowAllBreakables(0);
            EditorUtility.SetDirty(VehiclePartsController);
        }

        GUILayout.Label("Cargo");
        GUI.backgroundColor = Color.white;
        cargoIndex = EditorGUILayout.IntField("Index", cargoIndex);
        GUI.backgroundColor = Color.darkRed;
        if (GUILayout.Button("Show by Index"))
        {
            VehiclePartsController.ShowPartsByIndex(1, cargoIndex);
            EditorUtility.SetDirty(VehiclePartsController);
        }

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show All"))
        {
            VehiclePartsController.ShowAllParts(1);
            EditorUtility.SetDirty(VehiclePartsController);
        }

        GUI.backgroundColor = Color.white;
        cargoBreakableIndex = EditorGUILayout.IntField("Breakable Index", cargoBreakableIndex);
        GUI.backgroundColor = Color.darkRed;
        if (GUILayout.Button("Show Breakables by Index"))
        {
            VehiclePartsController.ShowBreakablesByIndex(1, cargoBreakableIndex);
            EditorUtility.SetDirty(VehiclePartsController);
        }

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show All Breakables"))
        {
            VehiclePartsController.ShowAllBreakables(1);
            EditorUtility.SetDirty(VehiclePartsController);
        }
    }
}


/*// Suspension
    Dictionary<string, string> lpSuspensionWheels = new Dictionary<string, string>()
    {
        { "LP_SSP", "suspension" },
        { "LP_WHL", "wheel" }
    };
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



        return;

       

        

        GUI.backgroundColor = Color.white;
        cargoBreakableIndex = EditorGUILayout.IntField("Cargo Breakable Index", cargoBreakableIndex);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show Cargo Breakables by Index"))
        {
            VehiclePartsController.ShowCargoBreakablesByIndex(cargoBreakableIndex);
            EditorUtility.SetDirty(VehiclePartsController);
        }
*/