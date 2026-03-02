using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2026 02 19
/// </summary>
public class WeaponController : MonoBehaviour
{
    List<WeaponSlot> allWeaponSlots = new List<WeaponSlot>();
    bool isFiring;
    bool isReloading;
    bool isTargeting;
    Vector3 position;


    void Update()
    {
        // Fire
        if (isFiring)
        {
            foreach (WeaponSlot weaponSlot in allWeaponSlots)
            {
                weaponSlot.weapon.Fire();
            }
        }

        // Reload
        if (isReloading)
        {
            foreach (WeaponSlot weaponSlot in allWeaponSlots)
            {
                weaponSlot.weapon.Reload();
            }
        }

        if (isTargeting)
        {
            foreach (WeaponSlot weaponSlot in allWeaponSlots)
            {
                weaponSlot.LookAtTarget(position);
            }
        }
    }

    public void SetFiring(bool value)
    {
        isFiring = value;
    }

    public void SetReloading(bool value)
    {
        isReloading = value;
    }

    public void SetTarget(bool value, Vector3 newPosition)
    {
        isTargeting = value;
        position = newPosition;
    }

    public void GetWeapons(List<WeaponSlot> weaponSlots)
    {
        allWeaponSlots.AddRange(weaponSlots);

        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            GameObject weapon = Instantiate(WeaponDatabase.Instance.GetRandomWeapon(), weaponSlot.transform);
            weaponSlot.weapon = weapon.GetComponent<Weapon>();
        }
    }
}