using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 2026 03 02
/// </summary>
public class PlayerInput : MonoBehaviour
{
    public RVP.VehicleParent vp;
    public string accelAxis = "Accel";
    public string brakeAxis = "Brake";
    public string steerAxis = "Horizontal";

    public WeaponController wc;
    public float distance = 100.0f;
    public LayerMask layerMask = 1;



    void Start()
    {
        // Player clone will disable moouse
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        // Vehicle
        if (!string.IsNullOrEmpty(accelAxis))
        {
            vp.SetAccel(Input.GetAxis(accelAxis));
        }

        if (!string.IsNullOrEmpty(brakeAxis))
        {
            vp.SetBrake(Input.GetAxis(brakeAxis));
        }

        if (!string.IsNullOrEmpty(steerAxis))
        {
            vp.SetSteer(Input.GetAxis(steerAxis));
        }

       
    }

    void Update()
    {
        // Weapons
        wc.SetFiring(Input.GetMouseButton(0));
        wc.SetReloading(Input.GetMouseButton(1));

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, distance, layerMask))
        {
            wc.SetTarget(true, hit.point);
        }
        else
        {
            // Prevent loosing target when shooting at long distance / or sky
            wc.SetTarget(true, ray.GetPoint(distance));
        }
    }
}
