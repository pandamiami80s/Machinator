using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "WeaponDB", menuName = "Data/WeaponDatabase")]
public class WeaponManager : ScriptableObject
{
    private static WeaponManager _instance;

    // This is the "Magic Link" other scripts use
    public static WeaponManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // This finds the file in your Resources folder automatically
                _instance = Resources.Load<WeaponManager>("WeaponDB");
            }
            return _instance;
        }
    }

    public List<GameObject> allWeapons = new List<GameObject>();

    // Call this to get a weapon by ID
    public GameObject GetWeapon(int id) => allWeapons[id];
}
/*
// This tiny block adds the "Update List" button to your Inspector
#if UNITY_EDITOR
[CustomEditor(typeof(WeaponManager))]
public class WeaponDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        WeaponManager db = (WeaponManager)target;

        if (GUILayout.Button("Update Weapon List from Folder"))
        {
            db.PopulateDatabase();
            EditorUtility.SetDirty(db); // Saves the changes to the file
        }
    }
}
#endif*/