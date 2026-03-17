using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[RequireComponent(typeof(Terrain))]
//[RequireComponent (typeof(TerrainCollider))]
/// <summary>
/// 2026 03 16
/// </summary>
public class TerrainGenerator : MonoBehaviour
{
    // Don't lose TerrainData!
    public Terrain terrain;
    public TerrainCollider terrainCollider;
    public int terrainDepth = 32;
    public int terrainSize = 128;
    public float noizeScale = 32.0f;



    public void GenerateChunk(Vector2 chunkIndex)
    {
        // Create unique terrainData per chunk
        TerrainData terrainData = new TerrainData();
        terrainData.name = "RuntimeTerrainData";
        // +1 for vertex
        int heightmapResolution = terrainSize + 1;
        terrainData.heightmapResolution = heightmapResolution;
        terrainData.size = new Vector3(terrainSize, terrainDepth, terrainSize);

        // Noize offset
        Vector2 chunkOffset = new Vector2(chunkIndex.x * terrainSize, chunkIndex.y * terrainSize);
        terrainData.SetHeights(0, 0, CalculateChunkHeights(heightmapResolution, chunkOffset));
        
        // Terrain "Paint"
        terrainData.terrainLayers = terrain.terrainData.terrainLayers;
        terrain.terrainData = terrainData;
        terrainCollider.terrainData = terrainData;

        terrain.Flush();
    }

    float[,] CalculateChunkHeights(int resolution, Vector2 chunkOffset)
    {
        float[,] heights = new float[resolution, resolution];
        // Unity stores terrain data using a (Z, X) 
        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float xNoise = (x + chunkOffset.x) / noizeScale;
                float zNoise = (z + chunkOffset.y) / noizeScale;

                heights[z, x] = Mathf.PerlinNoise(xNoise, zNoise);
            }
        }

        return heights;
    }

}

/*

    public void GenerateChunk(Vector2 chunkIndex)
    {
        // Create unique terrainData per chunk
        TerrainData terrainData = new TerrainData();
        terrainData.name = "RuntimeTerrainData";
        // Paint
        terrainData.terrainLayers = terrain.terrainData.terrainLayers;
        int heightmapResolution = terrainSize + 1;
        terrainData.heightmapResolution = heightmapResolution;


        // Move??
        terrainData.size = new Vector3(terrainSize, terrainDepth, terrainSize);
        Vector2 chunkOffset = new Vector2(chunkIndex.x * terrainSize, chunkIndex.y * terrainSize);


        terrainData.SetHeights(0, 0, CalculateChunkHeights(heightmapResolution, chunkOffset));
        terrain.terrainData = terrainData;
        terrain.Flush();

        
    }

    float[,] CalculateChunkHeights(int resolution, Vector2 chunkOffset)
    {
        float[,] heights = new float[resolution, resolution];
        // Unity stores terrain data using a (Z, X) 
        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {

                // Refactoring
                float xNoise = (x + chunkOffset.x) / noiseScale;
                float zNoise = (z + chunkOffset.y) / noiseScale;

                heights[z, x] = Mathf.PerlinNoise(xNoise, zNoise);
            }
        }

        return heights;
    }

*/

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    TerrainGenerator terrainGenerator => (TerrainGenerator)target;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Generate"))
        {
            terrainGenerator.GenerateChunk(new Vector2Int(0, 0));
        }
    }
}