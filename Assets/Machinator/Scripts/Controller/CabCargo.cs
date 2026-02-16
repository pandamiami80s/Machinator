using RVP;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 2026 02 04
///     Cab, Cargo breakables, lights, smoke etc
///     To do: Find LP_LIGHT, SMOKE etc
/// </summary>

public class CabCargo : MonoBehaviour
{/*
    [Serializable]
    public class Breakable
    {
        public List<Part> parts = new List<Part>();
    }
    [Serializable]
    public class Part
    {
        public GameObject gameObject;
        // If nothing make more simple
    }
    //[Header("Breakables")]
    public List<Breakable> breakables = new List<Breakable>();*/
    // Or Empty...


    public List<BreakableController> breakables = new List<BreakableController>();
    string breakableName = "Breakable";

    //[Header("Weapon Slot")]
    public List<WeaponSlot> weaponSlots = new List<WeaponSlot>();
    string[] lpWeaponNames = { "LP_SML" };





    /*

      public void DamagePart(int index, float damage)
    {
        if (index < 0 || index >= parts.Count) return;

        CabinPartGroup part = parts[index];

        float finalDamage = damage;

        if (part.currentStageIndex < 2)
        {
            finalDamage *= 0.7f; 
        }

        part.health -= finalDamage;

        if (globalHealth != null)
        {
            globalHealth.AddGlobalDamage(finalDamage);
        }

        if (part.currentStageIndex < part.stages.Length - 1)
        {
            int targetStage = 2 - Mathf.CeilToInt(part.health / 33.4f);
            targetStage = Mathf.Clamp(targetStage, 0, part.stages.Length - 1);

            if (targetStage > part.currentStageIndex)
            {
                part.stages[part.currentStageIndex].SetActive(false);
                part.currentStageIndex = targetStage;
                part.stages[targetStage].SetActive(true);
            }
        }
    }
}

    */

    public void SetCabAndCargo()
    {
        // Reset list back (don't forget)
        Undo.RecordObject(this, "Set Cab and Cargo");

        breakables.Clear();
        weaponSlots.Clear();

        // There are many ways to do this, sometime need i for index?
        // It is fine by now
        // Filter parents of parts by criteria
        List<Transform> parents = new List<Transform>();
        Transform[] allTransforms = transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform transformA in allTransforms)
        {
            if (transformA.name.Contains(breakableName))
            {
                if (transformA.GetChild(0).GetComponent<MeshRenderer>())
                {
                    parents.Add(transformA);
                }
            }

            // PREVENT DOUBLE CREATION
            foreach (string lpWeaponName in lpWeaponNames)
            {
                if (transformA.name.Contains(lpWeaponName))
                {
                    GameObject parent = new GameObject("new " + lpWeaponName);
                    Undo.RegisterCreatedObjectUndo(parent, "Set Cab Cargo");
                    Undo.SetTransformParent(parent.transform, transform, "Set Cab Cargo");

                    parent.transform.position = transformA.position;

                    // add
                    //Undo.RegisterFullObjectHierarchyUndo(transform, "Set Weapon Slots");

                    WeaponSlot weaponSlot = parent.GetComponent<WeaponSlot>();
                    if (weaponSlot == null)
                    {
                        weaponSlot = parent.AddComponent<WeaponSlot>();
                    }
                    weaponSlots.Add(weaponSlot);
                }
            }

                /* foreach (string lpWeaponName in lpWeaponNames)
                 {
                     if (transform.name.Contains(lpWeaponName))
                     {
                         Undo.RegisterFullObjectHierarchyUndo(transform, "Set Weapon Slots");

                         WeaponSlot weaponSlot = transform.GetComponent<WeaponSlot>();
                         if (weaponSlot == null)
                         {
                             weaponSlot = transform.AddComponent<WeaponSlot>();
                         }
                         weaponSlots.Add(weaponSlot);
                     }
                 }*/
        }

        foreach (Transform parent in parents)
        {
            BreakableController bc = parent.AddComponent<BreakableController>();
            bc.SetParts();
        }



            return;
        foreach (Transform parent in parents)
        {
            //Breakable breakable = new Breakable();
            for (int i = 0; i < parent.childCount; i++)
            {
                GameObject child = parent.GetChild(i).gameObject;
                
                Undo.RegisterFullObjectHierarchyUndo(child, "Set Cab and Cargo");
               
                // Components
                if (!child.GetComponent<BreakablePart>())
                {
                    BreakablePart bp = child.AddComponent<BreakablePart>();
                    //bp.cabCargo = this;
                }
                if (!child.GetComponent<MeshCollider>())
                {
                    child.AddComponent<MeshCollider>().convex = true;
                }

                // Data
                //Part part = new Part();
                //part.gameObject = child;
                //breakable.parts.Add(part);
            }
            //breakables.Add(breakable);
        }
        Debug.Log($"<color=yellow>Setup Complete:</color> {breakables.Count} breakables, {weaponSlots.Count} weapon slot(s)");
    }

    public void ShowBreakablesByIndex(int index)
    {
        if (breakables.Count <= 0)
        {
            Debug.LogError("Breakables list is empty");
            return;
        }

        if (index < 0 || breakables[0].parts.Count <= index)
        {
            Debug.LogError("Breakables index is out of range");

            return;
        }

      /*  foreach (Breakable breakable in breakables)
        {
            for (int i = 0; i < breakable.parts.Count; i++)
            {
                Undo.RecordObject(breakable.parts[i].gameObject, "Show Cab and Cargo Breakables by Index");
                breakable.parts[i].gameObject.SetActive(false);
            }
            
            /// out of range
            breakable.parts[index].gameObject.SetActive(true);
        }*/
    }

    public void ShowAllBreakables()
    {
        if (breakables.Count <= 0)
        {
            Debug.LogError("Breakables list is empty");
            return;
        }

       /* foreach (Breakable breakable in breakables)
        {
            for (int i = 0; i < breakable.parts.Count; i++)
            {
                Undo.RecordObject(breakable.parts[i].gameObject, "Show Cab and Cargo All Breakables");
                breakable.parts[i].gameObject.SetActive(true);
            }
        }*/
    }
}