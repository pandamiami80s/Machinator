using UnityEngine;
using System.Collections.Generic;
using System;

// создать empty для каждого сchild
// remove any anim clip




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

    // Метод для глубокого поиска
    public void SetupDeepHierarchy(string filterName)
    {
        parts.Clear();
        int partIndex = 0;

        // Находим вообще ВСЕ объекты в детях МАШИНЫ
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);

        foreach (Transform t in allChildren)
        {
            // Нам нужны только те, чье имя содержит "Empty"
            // Но мы исключаем сам корень и проверяем, есть ли внутри дети (меши)
            if (t.name.Contains(filterName) && t != transform && t.childCount > 0)
            {
                // Проверяем, что это именно "Родитель запчасти" (в нем должны быть меши, а не другие Empty)
                // Если первый ребенок имеет меш, значит это наша цель
                if (t.GetChild(0).GetComponent<MeshFilter>() != null || t.GetChild(0).name.ToLower().Contains("poly"))
                {
                    CabinPartGroup newGroup = new CabinPartGroup();
                    newGroup.name = t.name;

                    int stagesCount = t.childCount;
                    newGroup.stages = new GameObject[stagesCount];

                    for (int i = 0; i < stagesCount; i++)
                    {
                        GameObject stageObj = t.GetChild(i).gameObject;
                        newGroup.stages[i] = stageObj;

                        // Настройка DamageReceiver и MeshCollider на меш
                        SetupMeshObject(stageObj, partIndex);

                        stageObj.SetActive(i == 0);
                    }

                    parts.Add(newGroup);
                    partIndex++;
                }
            }
        }
        Debug.Log($"<color=green>Успех!</color> В глубокой иерархии найдено {partIndex} частей.");
    }

    private void SetupMeshObject(GameObject obj, int index)
    {
        foreach (var old in obj.GetComponents<DamageReceiver>()) DestroyImmediate(old);

        DamageReceiver dr = obj.AddComponent<DamageReceiver>();
        dr.partIndex = index;

        MeshCollider mc = obj.GetComponent<MeshCollider>();
        if (mc == null) mc = obj.AddComponent<MeshCollider>();
        mc.convex = true;
    }

    public void DamagePart(int index, float damage)
    {
        if (index < 0 || index >= parts.Count) return;

        CabinPartGroup part = parts[index];

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

        if (globalHealth != null)
        {
            globalHealth.AddGlobalDamage(finalDamage);
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
        }
    }
}