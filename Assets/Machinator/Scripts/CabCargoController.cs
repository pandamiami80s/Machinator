using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 2026 02 04
///     Cab, Cargo breakables, lights, smoke etc
///     To do: Find LP_LIGHT, SMOKE etc
/// </summary>

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


public class CabCargoController : MonoBehaviour
{
    public List<Breakable> breakables = new List<Breakable>();
    // Or Empty...
    string parentName = "Breakable";
    // Maybe list or class with LP type in future
    public List<Transform> turretLPs = new List<Transform>();
    string lpTurret = "LP_SML" ;



    public void SetBreakables()
    {
        // Reset list back (don't forget)
        Undo.RecordObject(this, "Set Cab and Cargo Breakables");

        breakables.Clear();

        // There are many ways to do this, sometime need i for index?
        // It is fine by now

        // Filter parents of parts by criteria
        List<Transform> parents = new List<Transform>();
        Transform[] allTransforms = transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform transform in allTransforms)
        {
            Transform parent = transform;
            if (parent.name.Contains(parentName))
            {
                if (parent.GetChild(0).GetComponent<MeshRenderer>())
                {
                    parents.Add(parent);
                }
            }
        }

        foreach (Transform parent in parents)
        {
            Breakable breakable = new Breakable();
            for (int i = 0; i < parent.childCount; i++)
            {
                GameObject child = parent.GetChild(i).gameObject;
                
                Undo.RegisterFullObjectHierarchyUndo(child, "Set Cab and Cargo Breakables");
               
                // Components
                if (!child.GetComponent<BreakablePart>())
                {
                    child.AddComponent<BreakablePart>();
                }
                if (!child.GetComponent<MeshCollider>())
                {
                    child.AddComponent<MeshCollider>().convex = true;
                }

                // Data
                Part part = new Part();
                part.gameObject = child;
                breakable.parts.Add(part);
            }
            breakables.Add(breakable);
        }
        Debug.Log($"<color=green>Setup Complete:</color> {breakables.Count} parents found");
    }

    public void SetTurrets()
    {
        Undo.RecordObject(this, "Set Turrets");

        turretLPs.Clear();



        // look at cargo

        // add turret slot script to LP use empty gameobject
        
        
        Transform[] allTransforms = transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform transform in allTransforms)
        {
            if (transform.name.Contains(lpTurret))
            {
                turretLPs.Add(transform);
            }
        }
    }

    public void ShowBreakablesByIndex(int index)
    {
        if (breakables.Count <= 0)
        {
            Debug.Log("List is empty");
            return;
        }

        if (index < 0 || breakables[0].parts.Count <= index)
        {
            Debug.Log("Index is out of range");

            return;
        }

        foreach (Breakable breakable in breakables)
        {
            for (int i = 0; i < breakable.parts.Count; i++)
            {
                Undo.RecordObject(breakable.parts[i].gameObject, "Show Cab and Cargo Breakables by Index");
                breakable.parts[i].gameObject.SetActive(false);
            }
            
            /// out of range
            breakable.parts[index].gameObject.SetActive(true);
        }
    }

    public void ShowAllBreakables()
    {
        if (breakables.Count <= 0)
        {
            Debug.Log("List is empty");
            return;
        }

        foreach (Breakable breakable in breakables)
        {
            for (int i = 0; i < breakable.parts.Count; i++)
            {
                Undo.RecordObject(breakable.parts[i].gameObject, "Show Cab and Cargo All Breakables");
                breakable.parts[i].gameObject.SetActive(true);
            }
        }
    }
}