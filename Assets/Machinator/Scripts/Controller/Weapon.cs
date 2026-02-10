using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 2026 02 07
/// </summary>

public class Weapon : MonoBehaviour
{
    [Header("Rotation")]
    // Accessed by TurretSLot
    public Transform turretX;
    public Transform turretY;
    string lpTurret = "LP_GUN";
    string gunName = "gun";
   

    [Header("Fire")]
    public GameObject bulletPrefab;
    public List<Transform> firePositions = new List<Transform>();
    string lpFire = "LP_FIRE";
    public GameObject shellPrefab;
    public List<Transform> shellPositions = new List<Transform>();
    string lpShell = "LP_SHELL";
   


    public void SetWeapon()
    {
        Undo.RecordObject(this, "Set Weapon");

        firePositions.Clear();
        shellPositions.Clear();

        Transform[] allTransforms = transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform transform in allTransforms)
        {
            // Find LP
            if (transform.name.Contains(lpTurret))
            {
                // Find gun
                foreach (Transform transformB in allTransforms)
                {
                    if (transformB.name.Contains(gunName))
                    {
                        turretY = transformB;

                        Undo.SetTransformParent(transformB, transform, "Set Weapon");
                        transformB.localPosition = Vector3.zero;
                    }
                }
            }

            // Fidn shell and fire
            if (transform.name.Contains(lpFire))
            {
                firePositions.Add(transform);
            }
            if (transform.name.Contains(lpShell))
            {
                shellPositions.Add(transform);
            }
        }

        Transform child = transform.GetChild(0);
        turretX = child;
        child.localPosition = Vector3.zero;
        Debug.Log($"<color=yellow>Setup Complete:</color> {firePositions.Count} fire position(s), {shellPositions.Count} shell position(s)");
    }

    public void Update()
    {
        Debug.DrawRay(firePositions[0].position, -firePositions[0].forward * 500000.0f, Color.red);
    }
}
