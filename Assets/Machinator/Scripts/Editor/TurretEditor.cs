using UnityEditor;
using UnityEngine;

/// <summary>
/// 2026 02 06
/// </summary>

[CustomEditor(typeof(TurretController))]
public class TurretEditor : Editor
{
    TurretController turretController => (TurretController)target;

    public override void OnInspectorGUI()
    {
        // Do not replace the entire Inspector view
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Set Turret"))
        {
           // turretController.SetCabAndCargo();
            EditorUtility.SetDirty(turretController);
        }


    }
}
