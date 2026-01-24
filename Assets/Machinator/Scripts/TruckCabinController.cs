using UnityEngine;
using System.Collections.Generic;

using System.Collections.Generic;
using UnityEngine;
using System;




[Serializable] // Обязательно, чтобы класс отображался в инспекторе Unity
public class CabinPartGroup
{
    public string name;            // Название части (например, "Door_Left")
    public GameObject[] stages;    // Массив из 3-х объектов: [0] целая, [1] помятая, [2] обломки
    public float health = 100f;    // Текущее здоровье конкретно этой части
    public int currentStageIndex = 0; // Индекс текущей активной модели
}



public class TruckCabinController : MonoBehaviour
{
    public List<CabinPartGroup> parts = new List<CabinPartGroup>();
    public EntityHealth globalHealth;

    public void SetupWithCollidersOnMeshes(string filterName)
    {
        parts.Clear();
        int partIndex = 0;

        foreach (Transform emptyTransform in transform)
        {
            // Ищем объекты по фильтру (например, Empty1, Empty2...)
            if (emptyTransform.name.Contains(filterName))
            {
                CabinPartGroup newGroup = new CabinPartGroup();
                newGroup.name = emptyTransform.name;

                int stagesCount = emptyTransform.childCount;
                newGroup.stages = new GameObject[stagesCount];

                for (int i = 0; i < stagesCount; i++)
                {
                    GameObject stageObj = emptyTransform.GetChild(i).gameObject;
                    newGroup.stages[i] = stageObj;

                    // --- НАСТРОЙКА МЕША ---

                    // Удаляем старые ресиверы, если есть
                    foreach (var old in stageObj.GetComponents<DamageReceiver>())
                        DestroyImmediate(old);

                    // Добавляем DamageReceiver прямо на меш
                    DamageReceiver dr = stageObj.AddComponent<DamageReceiver>();
                    dr.partIndex = partIndex; // Один индекс на все 3 стадии этой части

                    // Добавляем MeshCollider, если его нет
                    MeshCollider mc = stageObj.GetComponent<MeshCollider>();
                    if (mc == null) mc = stageObj.AddComponent<MeshCollider>();
                    mc.convex = true; // Обязательно для обнаружения столкновений пулями

                    // Включаем только первую модель
                    stageObj.SetActive(i == 0);
                }

                parts.Add(newGroup);
                partIndex++;
            }
        }
        Debug.Log($"Настройка завершена! Объектов Empty: {partIndex}. Коллайдеры и ресиверы на мешах.");
    }

    public void DamagePart(int index, float damage)
    {
        if (index < 0 || index >= parts.Count) return;

        CabinPartGroup part = parts[index];
        // Если уже на последней стадии и HP кончилось - выходим
        if (part.currentStageIndex >= part.stages.Length - 1 && part.health <= 0) return;

        part.health -= damage;
        if (globalHealth != null) globalHealth.AddGlobalDamage(damage);

        // Переключение стадий (0 -> 1 -> 2)
        int targetStage = 2 - Mathf.CeilToInt(part.health / 33.4f);
        targetStage = Mathf.Clamp(targetStage, 0, part.stages.Length - 1);

        if (targetStage > part.currentStageIndex)
        {
            part.stages[part.currentStageIndex].SetActive(false);
            part.currentStageIndex = targetStage;
            part.stages[targetStage].SetActive(true);
        }
    }
}