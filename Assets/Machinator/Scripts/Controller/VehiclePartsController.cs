using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

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

public class VehiclePartsController : MonoBehaviour
{
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

    [SerializeField] List<VehiclePart> vehicleParts = new List<VehiclePart>()
    {
        new VehiclePart("LP_CAB", "cab"),
        new VehiclePart("LP_BSK", "cargo")
    };



    Vector3 targetPoint;



    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Shooting
        foreach (VehiclePart vehiclePart in vehicleParts)
        {
            if (vehiclePart.lpTransform != null)
            {
                foreach (var weaponSlot in vehiclePart.lpTransform.GetChild(0).GetComponent<CabCargo>().weaponSlots)
                {
                    weaponSlot.LookAtTarget(targetPoint);

                    if (Input.GetMouseButtonDown(0))
                    {
                        weaponSlot.FireWeapon();
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        // Targeting
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
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
    }

    public void SetCabCargo()
    {
        Utils.FixModels(transform);
        string[] allLPs = vehicleParts.Select(p => p.lpName).ToArray();
        Utils.ExtractLoadPoints(transform, allLPs);

        // Set CabCargo
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
                            Undo.SetTransformParent(childB, childA, "Extract Load Points");
                            childB.localPosition = Vector3.zero;
                            // Script
                            if (!childB.GetComponent<CabCargo>())
                            {
                                CabCargo cabCargo = Undo.AddComponent<CabCargo>(childB.gameObject);
                                cabCargo.SetCabCargo();
                            }
                        }
                    }
                }
            }
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
    public LayerMask layerMask = 1;
   
    public WeaponManager weaponManager;












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
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Set Vehicle Parts"))
        {
            VehiclePartsController.SetCabCargo();
            EditorUtility.SetDirty(VehiclePartsController);
        }



        return;

       

        if (GUILayout.Button("Set Suspension And Wheels"))
        {
            VehiclePartsController.SetSuspensionAndWheels();
            EditorUtility.SetDirty(VehiclePartsController);
        }

        GUILayout.Space(10);
        GUILayout.Label("Debug");
        GUI.backgroundColor = Color.white;
        cabIndex = EditorGUILayout.IntField("Cab index", cabIndex);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show Cab by Index"))
        {
            VehiclePartsController.ShowCabByIndex(cabIndex);
            EditorUtility.SetDirty(VehiclePartsController);
        }

        GUI.backgroundColor = Color.white;
        cargoIndex = EditorGUILayout.IntField("Cargo index", cargoIndex);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show Cargo by Index"))
        {
            VehiclePartsController.ShowCargoByIndex(cargoIndex);
            EditorUtility.SetDirty(VehiclePartsController);
        }

        GUI.backgroundColor = Color.white;
        cabBreakableIndex = EditorGUILayout.IntField("Cab Breakable Index", cabBreakableIndex);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show Cab Breakables by Index"))
        {
            VehiclePartsController.ShowCabBreakablesByIndex(cabBreakableIndex);
            EditorUtility.SetDirty(VehiclePartsController);
        }

        GUI.backgroundColor = Color.white;
        cargoBreakableIndex = EditorGUILayout.IntField("Cargo Breakable Index", cargoBreakableIndex);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show Cargo Breakables by Index"))
        {
            VehiclePartsController.ShowCargoBreakablesByIndex(cargoBreakableIndex);
            EditorUtility.SetDirty(VehiclePartsController);
        }

        if (GUILayout.Button("Hide Suspension And Wheels"))
        {
            VehiclePartsController.HideSuspensionAndWheels();
            EditorUtility.SetDirty(VehiclePartsController);
        }
    }
}