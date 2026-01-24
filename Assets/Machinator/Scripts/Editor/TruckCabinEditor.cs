using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TruckCabinController))]
public class TruckCabinEditor : Editor
{
    string filter = "Empty";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TruckCabinController script = (TruckCabinController)target;

        GUILayout.Space(10);
        filter = EditorGUILayout.TextField("Имя родителя содержит:", filter);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("SETUP: Colliders on Meshes"))
        {
            Undo.RecordObject(script, "Setup Colliders on Meshes");
            script.SetupDeepHierarchy(filter);
            EditorUtility.SetDirty(script);
        }
    }
}