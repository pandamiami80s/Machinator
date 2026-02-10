using UnityEngine;
using UnityEditor;

/// <summary>
/// 2026 02 04
/// </summary>

[CustomEditor(typeof(VehiclePartsController))]
public class VehiclePartsEditor : Editor
{
    // Not to run in OnInspectorGUI every time
    VehiclePartsController vehiclePartsController => (VehiclePartsController)target;
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
        if (GUILayout.Button("Set Cab and Cargo"))
        {
            vehiclePartsController.SetCabAndCargo();
            EditorUtility.SetDirty(vehiclePartsController);
        }

        if (GUILayout.Button("Set Suspension And Wheels"))
        {
            vehiclePartsController.SetSuspensionAndWheels();
            EditorUtility.SetDirty(vehiclePartsController);
        }

        GUILayout.Space(10);
        GUILayout.Label("Debug");
        GUI.backgroundColor = Color.white;
        cabIndex = EditorGUILayout.IntField("Cab index", cabIndex);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show Cab by Index"))
        {
            vehiclePartsController.ShowCabByIndex(cabIndex);
            EditorUtility.SetDirty(vehiclePartsController);
        }

        GUI.backgroundColor = Color.white;
        cargoIndex = EditorGUILayout.IntField("Cargo index", cargoIndex);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show Cargo by Index"))
        {
            vehiclePartsController.ShowCargoByIndex(cargoIndex);
            EditorUtility.SetDirty(vehiclePartsController);
        }

        GUI.backgroundColor = Color.white;
        cabBreakableIndex = EditorGUILayout.IntField("Cab Breakable Index", cabBreakableIndex);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show Cab Breakables by Index"))
        {
            vehiclePartsController.ShowCabBreakablesByIndex(cabBreakableIndex);
            EditorUtility.SetDirty(vehiclePartsController);
        }

        GUI.backgroundColor = Color.white;
        cargoBreakableIndex = EditorGUILayout.IntField("Cargo Breakable Index", cargoBreakableIndex);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show Cargo Breakables by Index"))
        {
            vehiclePartsController.ShowCargoBreakablesByIndex(cargoBreakableIndex);
            EditorUtility.SetDirty(vehiclePartsController);
        }

        if (GUILayout.Button("Hide Suspension And Wheels"))
        {
            vehiclePartsController.HideSuspensionAndWheels();
            EditorUtility.SetDirty(vehiclePartsController);
        }
    }
}