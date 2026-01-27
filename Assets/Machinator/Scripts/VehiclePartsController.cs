using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

/// <summary>
/// 2026 01 27
///     Can add fucntion to just create and plus fill properties
/// </summary>

[Serializable]
public class ArmorPart
{
    public List<Armor> parts = new List<Armor>();
}

[Serializable]
public class Armor
{
    public GameObject gameObject;
    public int armor;
}



public class VehiclePartsController : MonoBehaviour
{
    public EntityHealth entityHealth;
    public int armor = 75;


    // Store states in one place
    public List<ArmorPart> armorParts = new List<ArmorPart>();

    

    /// <summary>
    /// Set coordinates for CAB AND CARGO/BASKET if presented
    /// </summary>
    /// <param name="lpCab"></param>
    /// <param name="lpCargo"></param>
    public void SetPartsCoordinates(string lpCab, string lpCargo)
    {
        string[] parentNames = { lpCab, lpCargo };
        int partCount = 0;

        foreach (string parentName in parentNames)
        {
            Transform[] transforms = transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in transforms)
            {
                Transform parent = child.parent;
                if (parent != null && parent.name.Contains(parentName))
                {
                    Undo.RecordObject(child, "Set Parts Coordinates");
                    child.localPosition = Vector3.zero;

                    partCount++;
                }
            }
        }

        Debug.Log($"<color=yellow>Coordinates set!</color> searched {partCount} load point(s)");
    }

    /// <summary>
    /// Set vehicle model to any damaged state
    /// </summary>
    /// <param name="partParentName"></param>
    /// <param name="partIndex"></param>
    public void SetupVehicleModel(string partParentName, int partIndex)
    {
        int partCount = 0;

        // Faster way to find vehicle parts
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            // Filter vehicle parts.
            // Depends on how you named it in blender LOL.
            // I recommend adding "Empty" parent to all poly_surfaces
            Transform transform = meshRenderer.transform;
            if (transform.parent.name.Contains(partParentName))
            {
                GameObject gameObject = transform.gameObject;
                Undo.RecordObject(gameObject, "Setup Vehicle Model");

                // Enable for reset case
                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }
                // Leave only chosen part visible
                if (transform.GetSiblingIndex() != partIndex)
                {
                    gameObject.SetActive(false);

                    partCount++;
                }
            }
        }
        Debug.Log($"<color=green>Setup vehicle parts:</color> searched {partCount} parts");
    }

    /// <summary>
    /// Set vehicle parts colliders and scripts
    /// </summary>
    /// <param name="partParentName"></param>
    public void SetupVehicleParts(string partParentName)
    {
        armorParts.Clear();
        int partCount = 0;
        // Reset list back (don't forget)
        Undo.RecordObject(this, "Setup Vehicle Parts A");

        // Filter parents of parts by criteria
        List<Transform> parents = new List<Transform>();
        foreach (Transform transform in GetComponentsInChildren<Transform>(true))
        {
            Transform parent = transform;
            if (parent.name.Contains(partParentName))
            {
                if (parent.GetChild(0).GetComponent<MeshRenderer>())
                {
                    parents.Add(parent);
                }
            }
        }

        foreach (Transform parent in parents)
        {
            ArmorPart armorPart = new ArmorPart();

            // Maybe need index in future?
            for (int i = 0; i < parent.childCount; i++)
            {
                GameObject child = parent.GetChild(i).gameObject;
                Undo.RegisterFullObjectHierarchyUndo(child, "Setup Vehicle Parts B");

                if (!child.GetComponent<VehiclePart>())
                {
                    VehiclePart vehiclePart = child.AddComponent<VehiclePart>();
                    vehiclePart.vehiclePartsController = this;
                }
                if (!child.GetComponent<MeshCollider>())
                {
                    child.AddComponent<MeshCollider>().convex = true;
                }
                
                // Data
                Armor part = new Armor();
                part.gameObject = child;
                part.armor = armor;
                armorPart.parts.Add(part);

                partCount++;
            }

            // 5. Add the full group (containing 3 parts) to the main list
            armorParts.Add(armorPart);
        }

        Debug.Log($"<color=green>Setup Complete:</color> {armorParts.Count} Groups created with {partCount} total parts.");
    }
   

   























    // damage class
    public void DamagePart(int damage)
    {
        //Debug.Log("DAMAGA");



        return;


        /*if (index < 0 || index >= armorParts.Count) return;

        ArmorParts part = armorParts[index];

        // Вычисляем финальный урон с учетом брони
        float finalDamage = damage;

        // Допустим, стадии 0 и 1 — это наличие брони
        // Если текущая стадия меньше 2, снижаем урон на 30%
        if (part.currentStageIndex < 2)
        {
            finalDamage *= 0.7f; // Умножаем на 0.7 (проходит только 70% урона)
        }

        // Применяем урон
        part.health -= finalDamage;

        if (entityHealth != null)
        {
            entityHealth.AddGlobalDamage(finalDamage);
        }

        // Проверяем, не пора ли сменить стадию визуально
        if (part.currentStageIndex < part.stages.Length - 1)
        {
            int targetStage = 2 - Mathf.CeilToInt(part.health / 33.4f);
            targetStage = Mathf.Clamp(targetStage, 0, part.stages.Length - 1);

            if (targetStage > part.currentStageIndex)
            {
                part.stages[part.currentStageIndex].SetActive(false);
                part.currentStageIndex = targetStage;
                part.stages[targetStage].SetActive(true);
            }
        }*/
    }
}