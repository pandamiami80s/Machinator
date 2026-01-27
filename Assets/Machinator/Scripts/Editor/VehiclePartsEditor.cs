using UnityEngine;
using UnityEditor;

/// <summary>
/// 2026 01 27
/// </summary>

[CustomEditor(typeof(VehiclePartsController))]
public class VehiclePartsEditor : Editor
{
    string lpCab = "LP_CAB";
    string lpCargo = "LP_BSK";
    string partParent = "Empty";
    int partIndex = 0;
    

    // Not to run in OnInspectorGUI every time
    VehiclePartsController vehiclePartsController => (VehiclePartsController)target;

    public override void OnInspectorGUI()
    {
        // Do not replace the entire Inspector view
        DrawDefaultInspector();

        GUILayout.Space(10);

        // Set coordinates
        lpCab = EditorGUILayout.TextField("Load Point Cab Name", lpCab);
        lpCargo = EditorGUILayout.TextField("Load Point Cargo Name", lpCargo);
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Set coordinates"))
        {
            vehiclePartsController.SetPartsCoordinates(lpCab, lpCargo);

            // Save changes on the scene
            EditorUtility.SetDirty(vehiclePartsController);
        }

        GUI.backgroundColor = Color.white;
        GUILayout.Space(10);

        // Set models
        partParent = EditorGUILayout.TextField("Armor Parent Name", partParent);
        partIndex = EditorGUILayout.IntField("Armor Index", partIndex);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Set vehicle model"))
        {
            vehiclePartsController.SetupVehicleModel(partParent, partIndex);
            
            // Save changes on the scene
            EditorUtility.SetDirty(vehiclePartsController);
        }

        // Set vehicle parts
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Set vehicle parts"))
        {
            vehiclePartsController.SetupVehicleParts(partParent);

            // Save changes on the scene
            EditorUtility.SetDirty(vehiclePartsController);
        }
    }
}