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
    int cargoIndex;
    int breakableIndex;


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
        cargoIndex = EditorGUILayout.IntField("Cargo index", cargoIndex);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show Cab and Cargo by Index"))
        {
            vehiclePartsController.ShowCabCargoByIndex(cabIndex, cargoIndex);
            EditorUtility.SetDirty(vehiclePartsController);
        }

        GUI.backgroundColor = Color.white;
        breakableIndex = EditorGUILayout.IntField("Breakable index", breakableIndex);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show Cab and Cargo Breakables by Index"))
        {
            vehiclePartsController.ShowCabCargoBreakablesByIndex(breakableIndex);
            EditorUtility.SetDirty(vehiclePartsController);
        }
    }
}