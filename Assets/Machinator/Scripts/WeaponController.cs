using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2026 02 19
/// </summary>
public class WeaponController : MonoBehaviour
{
    List<WeaponSlot> allWeaponSlots = new List<WeaponSlot>();

    [Header("Weapons")]
    public float distance = 100.0f;
    public LayerMask layerMask = 1;
    Vector3 targetPoint;

    public void GetWeapons(List<WeaponSlot> weaponSlots)
    {
        allWeaponSlots.AddRange(weaponSlots);

        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            GameObject weapon = Instantiate(WeaponDatabase.Instance.GetRandomWeapon(), weaponSlot.transform);
            weaponSlot.weapon = weapon.GetComponent<Weapon>();
        }
    }

    void Update()
    {
        // Fire
        if (Input.GetMouseButton(0))
        {
            foreach (WeaponSlot weaponSlot in allWeaponSlots)
            {
                weaponSlot.weapon.Fire();
            }
        }

        // Reload
        if (Input.GetMouseButtonDown(1))
        {
            foreach (WeaponSlot weaponSlot in allWeaponSlots)
            {
                weaponSlot.weapon.Reload();
            }
        }
    }

    void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, distance, layerMask))
        {
            targetPoint = hit.point;
        }
        else
        {
            // Prevent loosing target when shooting at long distance / or sky
            targetPoint = ray.GetPoint(distance);
        }
        Debug.DrawLine(ray.origin, targetPoint, Color.red);

        foreach (WeaponSlot weaponSlot in allWeaponSlots)
        {
            weaponSlot.LookAtTarget(targetPoint);
        }
    }
}