using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 2026 02 18
/// hitCountMax is number of hits to take before switching to next breakable part 
/// Can be armore points but need some rework. Armor will be stored at CabCargo and passed down to breakables
/// </summary>
public class BreakableController : MonoBehaviour
{
    public CabCargoController cabCargoController;
    public List<GameObject> breakableParts = new List<GameObject>();
    int hitCount;
    int hitCountMax = 3;
    int partIndex;



    public void SetBreakableParts()
    {
        // Does not redo
        Undo.RecordObject(this, "Set Breakable Parts");

        foreach (Transform child in transform)
        {
            GameObject gameObject = child.gameObject;
            if (!gameObject.GetComponent<BreakablePart>())
            {
                BreakablePart breakablePart = Undo.AddComponent<BreakablePart>(gameObject);
                breakablePart.breakableController = this;
            }
            if (!gameObject.GetComponent<MeshCollider>())
            {
                Undo.AddComponent<MeshCollider>(gameObject).convex = true;
            }
            breakableParts.Add(gameObject);
        }
    }

    public void DamagePart(int damage)
    {
        cabCargoController.vehiclePartsController.onDamaged.Invoke(damage);

        if (breakableParts.Count - 1 <= partIndex)
        {
            return;
        }

        hitCount++;
        if (hitCountMax <= hitCount)
        {
            breakableParts[partIndex].SetActive(false);
            partIndex++;
            breakableParts[partIndex].SetActive(true);

            hitCount = 0;
        }
    }
}