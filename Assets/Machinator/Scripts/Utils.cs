using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 2026 02 17
/// To import vehicles
/// </summary>
public static class Utils
{
    static Vector3 rotationOffset = new Vector3(0, 180.0f, 0);
    static string extractName = " extract";

    /// <summary>
    /// Thanks to blender need to create parent for models and rotate
    /// </summary>
    /// <param name="parent"></param>
    public static void FixModels(Transform parent)
    {
        string fixName = " model";
        List<Transform> rawModels = GetChildTransforms(parent);
        if (rawModels.Count <= 0)
        {
            return;
        }
        foreach (Transform child in parent)
        {
            if (child.name.Contains(fixName))
            {
                return;
            }
        }

        List<Transform> sortedModels = SortRawModels(parent, rawModels);
        foreach (Transform child in sortedModels)
        {
            GameObject newParent = ObjectFactory.CreateGameObject(child.name + fixName);
            Undo.RegisterCreatedObjectUndo(newParent, "Fix Models");
            newParent.transform.position = parent.position;
            Undo.SetTransformParent(newParent.transform, parent, "Fix Models");
            Undo.RecordObject(child, "Fix Models");
            Undo.SetTransformParent(child.transform, newParent.transform, "Fix Models");

            child.position = parent.position;
            child.Rotate(rotationOffset, Space.World);
        }
    }
    public static List<Transform> GetChildTransforms(Transform parent)
    {
        List<Transform> transforms = new List<Transform>();
        foreach (Transform child in parent)
        {
            transforms.Add(child);
        }

        return transforms;
    }

    static List<Transform> SortRawModels(Transform parent, List<Transform> models)
    {
        Undo.RegisterChildrenOrderUndo(parent, "Sort Raw Models");
        models.Sort((a, b) => a.name.CompareTo(b.name));
        // Update order in inspector
        for (int i = 0; i < models.Count; i++)
        {
            models[i].SetSiblingIndex(i);
        }

        return models;
    }

    /// <summary>
    /// Thanks to blender need to create parent for LP (Load Points)
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="lpNames"></param>
    public static void ExtractLoadPoints(Transform parent, string[] lpNames)
    {
        // Gets all LP including extracted on second button press
        List<Transform> loadPoints = GetAllLoadPoints(parent, lpNames);
        if (loadPoints == null)
        {
            return;
        }

        // Get children names: LP_CAB, LP_CAB extracted, LP_CARGO
        List<string> existingNames = parent.Cast<Transform>().Select(transform => transform.name).ToList();
        // Remove from list if contains any
        loadPoints.RemoveAll(loadPoint => existingNames.Any(existing => existing.Contains(loadPoint.name)));

        foreach (Transform loadPoint in loadPoints)
        {
            GameObject newParent = ObjectFactory.CreateGameObject(loadPoint.name + extractName);
            Undo.RegisterCreatedObjectUndo(newParent, "Extract Load Points");
            Undo.SetTransformParent(newParent.transform, parent, "Extract Load Points");
            newParent.transform.position = loadPoint.position;
        }
    }

    static List<Transform> GetAllLoadPoints(Transform parent, string[] lpNames)
    {
        List<Transform> loadPoints = new List<Transform>();
        Transform[] allTransforms = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allTransforms)
        {
            foreach (string lpName in lpNames)
            {
                if (child.name.Contains(lpName))
                {
                    loadPoints.Add(child);
                }
            }
        }

        return loadPoints;
    }
}