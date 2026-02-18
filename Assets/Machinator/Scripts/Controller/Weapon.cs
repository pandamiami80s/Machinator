using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 2026 02 07
/// </summary>

public class Weapon : MonoBehaviour
{


















    [Header("Rotation")]
    // Accessed by TurretSLot
    public Transform turretX;
    public Transform turretY;
    string[] lpNames = { "LP_GUN" } ;
    string gunName = "gun";
   

    [Header("Fire")]
    public GameObject bulletPrefab;
    public List<Transform> firePositions = new List<Transform>();
    string lpFire = "LP_FIRE";
    public GameObject shellPrefab;
    public List<Transform> shellPositions = new List<Transform>();
    string lpShell = "LP_SHELL";



    public Vector3 rotationOffset = new Vector3(0, 180.0f, 0);


    // hit?
    public void Fire()
    {
        Instantiate(bulletPrefab, firePositions[0].position, firePositions[0].rotation);

        //Debug.DrawLine(firePositions[0].position, hit, UnityEngine.Color.red, 1.0f);
    }


    // x = base
    // y = gun


    public void SetWeapon()
    {
        Utils.FixModels(transform);

        // Build weapon
        Transform gun = null;
        Transform gunBase = null;
        foreach (Transform child in transform)
        {
            if (child.name.Contains(gunName))
            {
                gun = child;
            }
            else
            {
                gunBase = child;
            }
        }
        
        if (gun != null && gunBase != null)
        {
            //Utils.FixLoadPoints(transform, lpNames);

            //List<Transform> loadPoint = Utils.GetLoadPoint(transform, lpNames);
           // Undo.SetTransformParent(loadPoint[0], gunBase.transform, "Set Weapon");
            //Undo.SetTransformParent(gun, loadPoint[0].transform, "Set Weapon");
            gun.localPosition = Vector3.zero;
        }




        return;





        Undo.RecordObject(this, "Set Weapon");

        firePositions.Clear();
        shellPositions.Clear();





        // Fix models by puting to empty ibjetcs
        List<GameObject> childList = new List<GameObject>();
        foreach (Transform childxx in transform)
        {
            childList.Add(childxx.gameObject);
        }
        // SORT index 01 03
        // Chagne to transform
        //childList.Sort((a, b) => a.name.CompareTo(b.name));

        foreach (GameObject childxx in childList)
        {
            GameObject parent = new GameObject("meow " + childxx.name);
            Undo.RegisterCreatedObjectUndo(parent, "Set Cab Cargo");
            Undo.SetTransformParent(parent.transform, transform, "Set Cab Cargo");
            Undo.SetTransformParent(childxx.transform, parent.transform, "Set Cab Cargo");

            childxx.transform.position = new Vector3();

            // ADD THIS T VEHICLE CONTROLLER
            //Vector3 currentEuler = childxx.transform.localEulerAngles;
            //Vector3 newEuler = currentEuler + rotationOffset;
            //childxx.transform.localEulerAngles = newEuler;
            childxx.transform.Rotate(rotationOffset, Space.World);
        }



        // finf lp and put gun on lp copy



        //Transform lp;
      

        /*// FIND LPS
        Transform[] allTransforms = transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform transformA in allTransforms)
        {
            // Find LP
            if (transformA.name.Contains(lpTurret))
            {
                GameObject parent = new GameObject("new " + lpTurret);
                Undo.RegisterCreatedObjectUndo(parent, "Set Cab Cargo");
                Undo.SetTransformParent(parent.transform, transform, "Set Cab Cargo");
                //Undo.SetTransformParent(transformA, parent.transform, "Set Cab Cargo");

                parent.transform.position = transformA.position;



                // MAKE GUN CHILD TO LP
                // NOT LIKE CAB LOGIC
                // gunName
                // OBJECT WITH LP THAW TIL NBE PARENT
                // FIND CHILD
                foreach (Transform transformB in allTransforms)
                {
                    if (transformB.name.Contains("meow") && transformB.name.Contains(gunName))
                    {
                        //Debug.Log(transformB.name);
                        //Debug.Log(parent.name);

                        Undo.SetTransformParent(transformB, parent.transform, "Set Cab Cargo");
                        transformB.localPosition = Vector3.zero;
                        turretY = transformB;
                    }

                    // Y to X
                    if (transformB.name.Contains("meow") && !transformB.name.Contains(gunName))
                    {
                        Debug.Log(transformB.name);
                        turretX = transformB;
                        Undo.SetTransformParent(parent.transform, transformB, "Set Cab Cargo");
                    }


                    //lp = parent.transform;
                }
            }

            // Fidn shell and fire
            if (transformA.name.Contains(lpFire))
            {
                firePositions.Add(transformA);
            }
            if (transformA.name.Contains(lpShell))
            {
                shellPositions.Add(transformA);
            }
        }*/





        // Put Y gun to X
        /*foreach (Transform transformB in transform)
        {
            if (transformB.name.Contains(gunName))
            {
                turretY = transformB;

                Undo.SetTransformParent(transformB, parent.transform, "Set Weapon");
                transformB.localPosition = Vector3.zero;
            }
        }*/

        // use LP to put gun





        // Buld weapon


        /*foreach (Transform transformB in transform)
        {
            if (transformB.name.Contains(gunName))
            {
                turretY = transformB;

                Undo.SetTransformParent(transformB, parent.transform, "Set Weapon");
                transformB.localPosition = Vector3.zero;
            }
        }*/

        //return;
        //Transform child = transform.GetChild(0);
        //turretX = child;
        //child.localPosition = Vector3.zero;
        //Debug.Log($"<color=yellow>Setup Complete:</color> {firePositions.Count} fire position(s), {shellPositions.Count} shell position(s)");
    }

    public void Update()
    {
        //Debug.DrawRay(firePositions[0].position, firePositions[0].forward * 500000.0f, Color.red);
    }
}


/// <summary>
/// 2026 02 07
/// </summary>

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
{
    Weapon Weapon => (Weapon)target;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Set Weapon"))
        {
            Weapon.SetWeapon();
            EditorUtility.SetDirty(Weapon);
        }
    }
}
