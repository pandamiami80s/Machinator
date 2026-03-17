using System.Collections.Generic;
using UnityEditor;
using UnityEngine;




[System.Serializable]
public class TerrainRoad
{
    public float width = 10f;       // Ширина ровного асфальта
    public float smoothness = 10f;  // Ширина плавного перехода/обочины
    public float curveScale = 0.005f; // Частота изгибов
    public float curveWidth = 60f;    // Сила отклонения
    [Range(0, 1)] public float textureSmoothness = 0.3f; // Насколько размыты края асфальта

}

[System.Serializable]
public class TerrainMountains
{
    public float mountainZoneStart = 40f;   // Где начинают расти горы от центра
    public float mountainSlopeWidth = 30f;  // На каком расстоянии они достигают пика
    public float mountainMaxHeight = 1.2f;  // Максимальная высота (может быть > 1.0, если Terrain.height позволяет)
    public float mountainMassiveScale = 80f; // ЧЕМ ВЫШЕ, ТЕМ КРУПНЕЕ И ШИРЕ ГОРЫ
    public float edgeDistortionStrength = 50f; // Сила "пляски" (чем выше, тем глубже выступы)
    public float edgeDistortionScale = 0.03f;  // Масштаб изгибов (мелкий или крупный)
    public float edgeRoughness = 0.15f;
}

[System.Serializable]
public class TerrainTree
{
    public GameObject treePrefab;
    [Range(0, 1)] public float treeDensity = 0.2f;
}




public class WorldGenerator : MonoBehaviour
{
    public GameObject terrainPrefab;

    // Need 100%
    public Transform player;
    public int viewDistance = 2;
    Dictionary<Vector2Int, GameObject> currentChunks = new Dictionary<Vector2Int, GameObject>();
    List<Vector2Int> chunksToRemove = new List<Vector2Int>();



    // FIX ME
    public int chunkSize = 256;






    void Update()
    {
        // Get player current chunk index
        Vector2Int playerChunk = new Vector2Int(Mathf.RoundToInt(player.position.x / chunkSize), Mathf.RoundToInt(player.position.z / chunkSize));

        // Generate chunks
        // Check visible distance for chunks
        for (int z = -viewDistance; z <= viewDistance; z++)
        {
            for (int x = -viewDistance; x <= viewDistance; x++)
            {
                Vector2Int visibleСhunk = new Vector2Int(playerChunk.x + x, playerChunk.y + z);
                if (!currentChunks.ContainsKey(visibleСhunk))
                {
                    GenerateChunk(visibleСhunk);
                }
            }
        }

        
        
        
        // Hide/destroy chunks
        chunksToRemove.Clear();

        foreach (var chunk in currentChunks)
        {
            // Считаем дистанцию в чанках (можно по метрам через Vector3.Distance)
            float distance = Vector2Int.Distance(playerChunk, chunk.Key);

            // Если чанк дальше радиуса видимости (+ запас 1, чтобы не мерцал на границе)
            if (distance > viewDistance + 1)
            {
                chunksToRemove.Add(chunk.Key);
            }
        }





        // Удаляем объекты и записи из словаря
        foreach (Vector2Int coord in chunksToRemove)
        {
            Destroy(currentChunks[coord]);
            currentChunks.Remove(coord);
        }
    }

    public void GenerateChunk(Vector2Int coord)
    {
        // chunk pos
        Vector3 spawnPos = new Vector3(coord.x * chunkSize, 0, coord.y * chunkSize);


        GameObject newChunk = Instantiate(terrainPrefab, spawnPos, Quaternion.identity, transform);
        TerrainGenerator terrainGenerator = newChunk.GetComponent<TerrainGenerator>();

        terrainGenerator.GenerateChunk(coord);



        currentChunks.Add(coord, newChunk);
    }




    public void GenerateChunks()
    {
        for (int z = -viewDistance; z <= viewDistance; z++)
        {
            for (int x = -viewDistance; x <= viewDistance; x++)
            {
                GameObject newChunk = Instantiate(terrainPrefab, Vector3.zero, Quaternion.identity, transform);
                TerrainGenerator terrainGenerator = newChunk.GetComponent<TerrainGenerator>();
                terrainGenerator.GenerateChunk(new Vector2(x,z));


                // MOve inside??
                newChunk.transform.position = new Vector3(x * terrainGenerator.terrainSize, 0, z * terrainGenerator.terrainSize);
            }
        }
    }

    public void ClearChunks()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}

/*
    public void GenerateChunks()
    {
        for (int z = 0; z < 3; z++)
        {
            for (int x = 0; x < 3; x++)
            {
                GameObject newChunk = Instantiate(terrainPrefab, Vector3.zero, Quaternion.identity, transform);
                TerrainGenerator terrainGenerator = newChunk.GetComponent<TerrainGenerator>();
                terrainGenerator.GenerateChunk(new Vector2(x,z));


                // MOve inside??
                newChunk.transform.position = new Vector3(x * terrainGenerator.terrainSize, 0, z * terrainGenerator.terrainSize);
            }
        }
    }

    public void ClearChunks()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
*/

[CustomEditor(typeof(WorldGenerator))]
public class WorldGeneratorEditor : Editor
{
    WorldGenerator script => (WorldGenerator)target;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
       
        GUILayout.Space(10);
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Generate chunks"))
        {
            script.GenerateChunks();
        }

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Clear chunks"))
        {
            script.ClearChunks();
        }
    }
}