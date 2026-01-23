using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ArmorPart : MonoBehaviour, IDamageable
{
    public int armor = 100;
    int armorIndex;

    public List<GameObject> armorList = new List<GameObject>();



    public void TakeDamage(int amount)
    {
        Debug.Log("Armor damage taken " + amount);

        armor -= amount;

        if (armorList.Count - 1 <= armorIndex)
        {
            Debug.Log("Armor depleted");

            return;
        }

        if (armor <= 0)
        {
            armorList[armorIndex].SetActive(false);
            armorIndex++;
            armorList[armorIndex].SetActive(true);
            armor = 100;
        }
    }
}
