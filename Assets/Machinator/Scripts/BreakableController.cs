using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BreakableController : MonoBehaviour
{
    public List<GameObject> parts = new List<GameObject>();

    // damage index
    int damageIndex;

    public void SetParts()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;

            Undo.RegisterFullObjectHierarchyUndo(child, "Set Cab and Cargo");

            // Components
            if (!child.GetComponent<BreakablePart>())
            {
                BreakablePart bp = child.AddComponent<BreakablePart>();
                bp.bc = this;
            }
            if (!child.GetComponent<MeshCollider>())
            {
                child.AddComponent<MeshCollider>().convex = true;
            }

            // Data
            //Part part = new Part();
            //part.gameObject = child;
            parts.Add(child);
        }
    }

    private void Start()
    {
        // init
        foreach (GameObject part in parts)
        {
            part.SetActive(false);
        }

        parts[0].SetActive(true);
    }


    public void CalcDamage(int amount)
    {
        if (parts.Count-1 <= damageIndex)
        {
            return;
        }

        // find selt in a slist
        // Who was damged
        Debug.Log(0);

        

       

        parts[damageIndex].SetActive(false);
        damageIndex++;
        parts[damageIndex].SetActive(true);
        // partr index is from that list
    }


}
