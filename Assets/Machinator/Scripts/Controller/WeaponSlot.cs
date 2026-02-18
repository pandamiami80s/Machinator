using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 2026 02 07
/// </summary>

public class WeaponSlot : MonoBehaviour
{
    public Weapon weapon;

    [Header("Rotation")]
    public bool isRearMounted;
    public float minY = -180f;
    public float maxY = 180f;
    public float minX = -15.0f;
    public float maxX = 15.0f;
    Vector2 currentAngles;
    public float rotationSpeed = 90f;
    float gizmoDrawDistance = 2.0f;

    //public Vector3 localOffset = new Vector3(-90.0f, 180.0f, 0.0f);


    public void FireWeapon()
    {
        weapon.Fire();
    }

    public void SpawnWeapon()
    {
        //weapon = transform.GetChild(0).GetComponent<Weapon>();

        // No dragging! We just call WeaponDatabase.Instance
        GameObject prefab = WeaponManager.Instance.GetWeapon(0);

        if (prefab != null)
        {
            GameObject weaponss = Instantiate(prefab, transform.position, transform.rotation);
            weaponss.transform.SetParent(transform);

            weapon = weaponss.GetComponent<Weapon>();
        }
    }

    void Start()
    {
        SpawnWeapon();
    }

    public void LookAtTarget(Vector3 targetPoint)
    {
        // Get local direction to target
        // Was -transform.position oir turret base position idk now
        Vector3 direction = targetPoint - weapon.turretX.position;
        Vector3 localDirection = transform.InverseTransformDirection(direction);

        // add for child 
        //localDirection = Quaternion.Euler(localOffset) * localDirection;

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
        weapon.turretX.localRotation = Quaternion.Euler(0, finalRotation, 0);

        // Yaw
        // Inversed
        float groundDistance = new Vector2(localDirection.x, localDirection.z).magnitude;
        // default:
        float targetPitch = -Mathf.Atan2(localDirection.y, groundDistance) * Mathf.Rad2Deg;
        float clampedPitch = Mathf.Clamp(targetPitch, minX, maxX);
        currentAngles.x = Mathf.MoveTowards(currentAngles.x, clampedPitch, rotationSpeed * Time.fixedDeltaTime);
        weapon.turretY.localRotation = Quaternion.Euler(currentAngles.x, 0, 0);
    }

    //OnDrawGizmosSelected
    void OnDrawGizmos()
    {
        float totalAngle = maxY - minY;
        Color color = Color.red;
        if (0 < totalAngle && totalAngle <= 360.0f)
        {
            color = Color.green;
        }

        Vector3 forward = Vector3.forward;
        if (isRearMounted)
        {
            forward = -Vector3.forward;
        }

        Gizmos.color = color;
        Handles.color = color;

        Vector3 direction = Quaternion.AngleAxis(minY, Vector3.up) * forward;
        Vector3 directionB = Quaternion.AngleAxis(maxY, Vector3.up) * forward;
        Gizmos.DrawRay(transform.position, direction * gizmoDrawDistance);
        Gizmos.DrawRay(transform.position, directionB * gizmoDrawDistance);

        Handles.DrawWireArc(transform.position, Vector3.up, direction, totalAngle, gizmoDrawDistance);
        Handles.color = color * new Color(1, 1, 1, 0.1f);
        Handles.DrawSolidArc(transform.position, Vector3.up, direction, totalAngle, gizmoDrawDistance);

        Gizmos.color = Color.yellow;
        float bisectorAngle = (minY + maxY) / 2f;
        Vector3 bisectorDir = Quaternion.AngleAxis(bisectorAngle, Vector3.up) * forward;
        Gizmos.DrawRay(transform.position, bisectorDir * gizmoDrawDistance);
    }
}