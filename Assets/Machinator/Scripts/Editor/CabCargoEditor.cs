using UnityEditor;
using UnityEngine;

/// <summary>
/// 2026 02 03
/// </summary>

[CustomEditor(typeof(CabCargoController))]
public class CabCargoEditor : Editor
{
    CabCargoController cabCargoController => (CabCargoController)target;
    int breakableIndex;



    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Set Cab and Cargo Breakables"))
        {
            cabCargoController.SetBreakables();
            EditorUtility.SetDirty(cabCargoController);
        }

        // Debug part (If brekables list is full)
        GUILayout.Space(10);
        GUILayout.Label("Debug");
        GUI.backgroundColor = Color.white;
        breakableIndex = EditorGUILayout.IntField("Breakable index", breakableIndex);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show Cab and Cargo Breakables by Index"))
        {
            cabCargoController.ShowBreakablesByIndex(breakableIndex);
            EditorUtility.SetDirty(cabCargoController);
        }
        if (GUILayout.Button("Show Cab and Cargo All Breakables"))
        {
            cabCargoController.ShowAllBreakables();
            EditorUtility.SetDirty(cabCargoController);
        }
    }
}