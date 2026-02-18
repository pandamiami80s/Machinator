using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 2026 02 04
///     Cab, Cargo breakables, lights, smoke etc
///     To do: Find LP_LIGHT, SMOKE etc
/// </summary>

public class CabCargo : MonoBehaviour
{
    string breakableName = "Breakable";

    public List<WeaponSlot> weaponSlots = new List<WeaponSlot>();
    string[] lpWeaponNames = { "LP_SML" };



    public void SetCabCargo()
    {
        Undo.RecordObject(this, "Set Cab And Cargo");

        Utils.ExtractLoadPoints(transform, lpWeaponNames);

        SetBreakables();

        SetWeaponSlots();
    }

    void SetBreakables()
    {
        Undo.RecordObject(this, "Set Breakables");
        
        foreach (Transform child in transform.GetChild(0))
        {
            //0 < child.childCount
            if (child.name.Contains(breakableName))
            {
                if (child.GetChild(0).GetComponent<MeshRenderer>())
                {
                    if (!child.GetComponent<BreakableController>())
                    {
                        BreakableController breakableController = Undo.AddComponent<BreakableController>(child.gameObject);
                        breakableController.SetBreakableParts();
                    }
                }
            }
        }
    }

    void SetWeaponSlots()
    {
        // Weapons
        weaponSlots.Clear();

        foreach (Transform child in transform)
        {
            foreach (string lpWeaponName in lpWeaponNames)
            {
                if (child.name.Contains(lpWeaponName))
                {
                    WeaponSlot weaponSlot = child.GetComponent<WeaponSlot>();
                    if (weaponSlot == null)
                    {
                        weaponSlot = Undo.AddComponent<WeaponSlot>(child.gameObject);
                    }
                    weaponSlots.Add(weaponSlot);
                }
            }
        }
    }

    public void ShowBreakablesByIndex(int index)
    {
        if (index < 0)
        {
            return;
        }

        foreach (Transform child in transform.GetChild(0))
        {
            if (child.TryGetComponent(out BreakableController breakableController))
            {
                foreach (GameObject gameObject in breakableController.breakableParts)
                {
                    Undo.RecordObject(gameObject, "Show Breakables by Index");
                    gameObject.SetActive(false);
                }

                breakableController.breakableParts[index].SetActive(true);
            }
        }
    }

    public void ShowAllBreakables()
    {
        foreach (Transform child in transform.GetChild(0))
        {
            if (child.TryGetComponent(out BreakableController breakableController))
            {
                foreach (GameObject gameObject in breakableController.breakableParts)
                {
                    Undo.RecordObject(gameObject, "Show All Breakables");
                    gameObject.SetActive(true);
                }
            }
        }
    }
}

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
        if (GUILayout.Button("Set Cab Cargo"))
        {
            CabCargo.SetCabCargo();
            EditorUtility.SetDirty(CabCargo);
        }
        
        // Debug part (If brekables list is full)
        GUILayout.Space(10);
        GUILayout.Label("Debug");
        GUI.backgroundColor = Color.white;
        breakableIndex = EditorGUILayout.IntField("Breakable index", breakableIndex);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show Breakables by Index"))
        {
            CabCargo.ShowBreakablesByIndex(breakableIndex);
            EditorUtility.SetDirty(CabCargo);
        }

        if (GUILayout.Button("Show All Breakables"))
        {
            CabCargo.ShowAllBreakables();
            EditorUtility.SetDirty(CabCargo);
        }
    }
}