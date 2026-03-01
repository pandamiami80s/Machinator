using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 2026 02 04
///     Cab, Cargo breakables, lights, smoke etc
///     To do: Find LP_LIGHT, SMOKE etc
/// </summary>

public class CabCargoController : MonoBehaviour
{
    public VehiclePartsController vehiclePartsController;
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
                        breakableController.cabCargoController = this;
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

[CustomEditor(typeof(CabCargoController))]
public class CabCargoEditor : Editor
{
    CabCargoController CabCargoController => (CabCargoController)target;
    int breakableIndex;



    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUILayout.Label("Editor");
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Set Cab Cargo"))
        {
            CabCargoController.SetCabCargo();
            EditorUtility.SetDirty(CabCargoController);
        }
       
        GUI.backgroundColor = Color.white;
        breakableIndex = EditorGUILayout.IntField("Breakable index", breakableIndex);
        GUI.backgroundColor = Color.darkRed;
        if (GUILayout.Button("Show Breakables by Index"))
        {
            CabCargoController.ShowBreakablesByIndex(breakableIndex);
            EditorUtility.SetDirty(CabCargoController);
        }

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Show All Breakables"))
        {
            CabCargoController.ShowAllBreakables();
            EditorUtility.SetDirty(CabCargoController);
        }
    }
}