using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 2026 02 24
///     Put to "Resources/Weapons" folder
/// </summary>
[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Data/WeaponDatabase")]
public class WeaponDatabase : ScriptableObject
{
    static WeaponDatabase weaponDatabase;
    public static WeaponDatabase Instance
    {
        get
        {
            if (weaponDatabase == null)
            {
                weaponDatabase = Resources.Load<WeaponDatabase>("Weapons/WeaponDatabase");
            }
            return weaponDatabase;
        }
    }




    // List
    public List<GameObject> allWeapons = new List<GameObject>();
    
    public GameObject GetRandomWeapon()
    {
        int index = Random.Range(0, allWeapons.Count);
        return allWeapons[index];
    }

    public GameObject GetWeaponByName(string weaponName)
    {
        GameObject weapon = allWeapons.FirstOrDefault(w => w.name == weaponName);

        return weapon;
    }
}