using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 2026 02 07
///     Always two, there are. No more. No less. A gunBase and a gun
/// </summary>

public class Weapon : MonoBehaviour
{
    [Header("Rotation")]
    public Transform gunBase;
    public Transform gun;
    Transform lpGun;
    string[] lpNames = { "LP_GUN" };
    string gunName = "gun";

    [Header("Fire")]
    public List<Transform> firePositions = new List<Transform>();
    public GameObject bulletPrefab;
    string[] lpFire = { "LP_FIRE" };
    public List<Transform> shellPositions = new List<Transform>();
    public GameObject shellPrefab;
    string[] lpShell = { "LP_SHELL" };



    public void SetWeapon()
    {
        Utils.FixModels(transform);
        // Because LP is moved logic is not working
        if (gunBase != null)
        {
            return;
        }
        Utils.ExtractLoadPoints(transform, lpNames);

        // Set weapon load points
        foreach (Transform child in transform)
        {
            if (child.name.Contains(gunName))
            {
                gun = child;
            }
            // Always one
            else if (child.name.Contains(lpNames[0]))
            {
                lpGun = child;
            }
            else
            {
                gunBase = child;
            }
        }
        if (gun && gunBase && lpGun)
        {

            // Second loop is checkign for extracted and stops
            firePositions = SetGunLoadPoints(lpFire);
            shellPositions = SetGunLoadPoints(lpShell);

            Undo.SetTransformParent(lpGun, gunBase, "Set Weapon");
            Undo.SetTransformParent(gun, lpGun, "Set Weapon");

            gun.localPosition = Vector3.zero;
        }
    }

    List<Transform> SetGunLoadPoints(string[] lpName)
    {
        List<Transform> loadPoints = new List<Transform>();
        Utils.ExtractLoadPoints(gun, lpName);
        foreach (Transform child in gun)
        {
            Debug.Log(child.name + " " + lpName[0]);
            // Always one
            if (child.name.Contains(lpName[0]))
            {
                loadPoints.Add(child);
            }
        }

        return loadPoints;
    }

    public void Fire()
    {
        Instantiate(bulletPrefab, firePositions[0].position, firePositions[0].rotation);
    }
}

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
{
    Weapon Weapon => (Weapon)target;



    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUILayout.Label("Editor");
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Set Weapon"))
        {
            Weapon.SetWeapon();
            EditorUtility.SetDirty(Weapon);
        }
    }
}
