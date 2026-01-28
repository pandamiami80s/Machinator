using UnityEditor;
using UnityEngine;

/// <summary>
/// 2026 01 28
///     Turret rotated in a slot along Y and X (Adjustable angles)
/// </summary>

public class TurretSlot : MonoBehaviour
{
    public Transform turretBase;
    public Transform turretBarrel;

    // Can remove? Just for ease of use
    public bool isRearMounted;
    public float minY = -90f;
    public float maxY = 45f;
    public float minX = -15.0f;
    public float maxX = 15.0f;
    Vector2 currentAngles;
    public float rotationSpeed = 90f;

    float gizmoDrawDistance = 2.0f;

    public void LookAtTarget(Vector3 targetPoint)
    {
        // Get local direction to target
        Vector3 direction = targetPoint - turretBase.position;
        Vector3 localDirection = transform.InverseTransformDirection(direction);
        
        // Rear mounted case (Peacekeeper Phantom slot)
        if (isRearMounted)
        {
            localDirection = -localDirection;
        }
        
        // Get angle degree
        float targetYaw = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
        
        // Is full range or clamp used?
        // Different logic used here, dont ask why, it just works
        float totalRange = maxY - minY;
        if (360f <= totalRange)
        {
            // Shortest path for turret
            float delta = Mathf.DeltaAngle(currentAngles.y, targetYaw);
            currentAngles.y += Mathf.MoveTowards(0, delta, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Dont allow shortest path throuh a dead zone
            float center = (minY + maxY) / 2f;
            float halfRange = totalRange / 2f;
            // Also rotate turret throuhg zone center, not slot/truck
            float deltaFromCenter = Mathf.DeltaAngle(center, targetYaw);
            float clampedTarget = center + Mathf.Clamp(deltaFromCenter, -halfRange, halfRange);
            currentAngles.y = Mathf.MoveTowards(currentAngles.y, clampedTarget, rotationSpeed * Time.fixedDeltaTime);
        }

        float finalRotation;
        if (isRearMounted == true)
        {
            finalRotation = currentAngles.y + 180f;
        }
        else
        {
            finalRotation = currentAngles.y;
        }
        turretBase.localRotation = Quaternion.Euler(0, finalRotation, 0);

        // Yaw
        // Inversed
        float targetPitch = -Mathf.Atan2(localDirection.y, localDirection.z) * Mathf.Rad2Deg;
        float clampedPitch = Mathf.Clamp(targetPitch, minX, maxX);
        currentAngles.x = Mathf.MoveTowards(currentAngles.x, clampedPitch, rotationSpeed * Time.fixedDeltaTime);
        turretBarrel.localRotation = Quaternion.Euler(currentAngles.x, 0, 0);
    }

    //OnDrawGizmosSelected
    void OnDrawGizmos()
    {
        if (!turretBase)
        {
            return;
        }

        float totalAngle = maxY - minY;
        Color color = Color.red;
        if (0 < totalAngle && totalAngle <= 360.0f)
        {
            color = Color.green;
        }

        Vector3 forward = transform.forward;
        if (isRearMounted)
        {
            forward = -transform.forward;
        }

        Gizmos.color = color;
        Handles.color = color;

        Vector3 direction = Quaternion.AngleAxis(minY, transform.up) * forward;
        Vector3 directionB = Quaternion.AngleAxis(maxY, transform.up) * forward;
        Gizmos.DrawRay(turretBase.position, direction * gizmoDrawDistance);
        Gizmos.DrawRay(turretBase.position, directionB * gizmoDrawDistance);
       
        Handles.DrawWireArc(turretBase.position, transform.up, direction, totalAngle, gizmoDrawDistance);
        Handles.color = color * new Color(1, 1, 1, 0.1f);
        Handles.DrawSolidArc(turretBase.position, transform.up, direction, totalAngle, gizmoDrawDistance);

        Gizmos.color = Color.yellow;
        float bisectorAngle = (minY + maxY) / 2f;
        Vector3 bisectorDir = Quaternion.AngleAxis(bisectorAngle, transform.up) * forward;
        Gizmos.DrawRay(turretBase.position, bisectorDir * gizmoDrawDistance);
    }
}