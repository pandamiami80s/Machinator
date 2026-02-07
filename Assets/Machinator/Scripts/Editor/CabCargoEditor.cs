using UnityEditor;
using UnityEngine;

/// <summary>
/// 2026 02 03
/// </summary>

[CustomEditor(typeof(CabCargo))]
public class CabCargoEditor : Editor
{
    CabCargo CabCargo => (CabCargo)target;
    int breakableIndex;



    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Set Cab and Cargo"))
        {
            CabCargo.SetCabAndCargo();
            EditorUtility.SetDirty(CabCargo);
        }

        // Debug part (If brekables list is full)
        GUILayout.Space(10);
        GUILayout.Label("Debug");
        GUI.backgroundColor = Color.white;
        breakableIndex = EditorGUILayout.IntField("Breakable index", breakableIndex);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show Cab and Cargo Breakables by Index"))
        {
            CabCargo.ShowBreakablesByIndex(breakableIndex);
            EditorUtility.SetDirty(CabCargo);
        }

        if (GUILayout.Button("Show Cab and Cargo All Breakables"))
        {
            CabCargo.ShowAllBreakables();
            EditorUtility.SetDirty(CabCargo);
        }
    }
}