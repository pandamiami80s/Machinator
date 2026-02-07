using UnityEditor;
using UnityEngine;

/// <summary>
/// 2026 02 07
/// </summary>

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
{
    Weapon Weapon => (Weapon)target;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Set Weapon"))
        {
            Weapon.SetWeapon();
            EditorUtility.SetDirty(Weapon);
        }
    }
}
