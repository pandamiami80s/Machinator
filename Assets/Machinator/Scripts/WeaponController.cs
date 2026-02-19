using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2026 02 19
/// </summary>
public class WeaponController : MonoBehaviour
{
    // Check weapon and slots logic
    // Setup weapon editor
    // Check projectiles
    // Procceed to lP_light maybe
    // Need all waepon slots here and update them on cab changed? event??? 
    // debugger get first actiavqated object here
    // real one gets when cab is created
    // get slot -> slot get weapon
    
    
    
    // HOW TO FILL IT?
    public List<WeaponSlot> weaponSlots = new List<WeaponSlot>();
   



    [Header("Weapons")]
    public float distance = 100.0f;
    public LayerMask layerMask = 1;


    Vector3 targetPoint;



    void Start()
    {
        // Get weapon slots on current cab
        
        

        // Spawn weapon
        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            GameObject weapon = Instantiate(WeaponManager.Instance.GetWeapon(0), weaponSlot.transform);
            weaponSlot.weapon = weapon.GetComponent<Weapon>();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (WeaponSlot weaponSlot in weaponSlots)
            {
                weaponSlot.weapon.Fire();
            }
        }
    }

    void FixedUpdate()
    {
        // Targeting
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

        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            weaponSlot.LookAtTarget(targetPoint);
        }
    }
}