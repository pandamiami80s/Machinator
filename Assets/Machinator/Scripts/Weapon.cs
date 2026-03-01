using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

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

    [Header("")]
    public int shotCount = 1;
    public float spread = 1.0f;
    // Two times less than irl
    public float roundsPerMinute = 300f;
    float timeBetweenShots;
    float nextFireTime;
   
    // Ammo
    public int maxAmmo = 10;
    int currentAmmo;
    public float reloadTime = 2.0f;
    bool isReloading;

    [Header("Effects")]
    public AudioSource audioSource;
    public AudioClip audioClip;
    


    void Start()
    {
        timeBetweenShots = 60f / roundsPerMinute;
        nextFireTime = Time.time;
        currentAmmo = maxAmmo;
    }

    public void Fire()
    {
        if (isReloading)
        {
            return;
        }

        if (currentAmmo <= 0)
        {
            Reload();

            return;
        }

        if (nextFireTime <= Time.time)
        {
            audioSource.PlayOneShot(audioClip);

            foreach (Transform firePosition in firePositions)
            {
                for (int i = 0; i < shotCount; i++)
                {
                    Vector2 randomRadius = Random.insideUnitCircle * spread;
                    Quaternion randomSpread = Quaternion.Euler(randomRadius.x, randomRadius.y, 0);
                    Instantiate(bulletPrefab, firePosition.position, firePosition.rotation * randomSpread);
                }
            }

            currentAmmo--;
            nextFireTime = Time.time + timeBetweenShots;
        }
    }

    public void Reload()
    {
        if (!isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(IEReload());
        }
    }

    IEnumerator IEReload()
    {
        //Debug.Log("Reloading " + transform.gameObject.name);
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
    }

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
