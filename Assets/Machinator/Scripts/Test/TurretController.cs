using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 2025 01 28
///     Control all turrets in one script
/// </summary>

public class TurretController : MonoBehaviour
{
    public List<TurretSlot> turretSlots = new List<TurretSlot>();
    // Approximate distance for target calculations (Max view distance recommended)
    public float distance = 100.0f;
    public LayerMask layerMask;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, distance, layerMask))
        {
            targetPoint = hit.point;
        }
        else
        {
            // Prevent loosing target when shooting at long distance / or sky
            targetPoint = ray.GetPoint(distance);
        }

        foreach (var slot in turretSlots)
        {
            slot.LookAtTarget(targetPoint);
        }
    }
}
