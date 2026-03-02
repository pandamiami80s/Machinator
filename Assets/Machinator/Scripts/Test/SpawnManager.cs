using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public CinemachineCamera camera;

    public GameObject enemyPrefab;
    public int enemyCount = 8;
    public Transform spawnPointContainer;
    float spawnRadius = 100.0f;
    List<Transform> spawnPoints = new List<Transform>();




    void Start()
    {
        // Spawn player
        GameObject player = Instantiate(playerPrefab, new Vector3(0, 2, 0), Quaternion.identity);
        player.GetComponent<RVP.FollowAI>().enabled = false;
        player.GetComponent<TargetAI>().enabled = false;
        camera.Follow = player.transform;

        // Spawn enemies
        //return;
        for (int i = 0; i < enemyCount; i++)
        {
            float angle = i * Mathf.PI * 2 / enemyCount;
            float x = Mathf.Cos(angle) * spawnRadius;
            float z = Mathf.Sin(angle) * spawnRadius;

            Vector3 pos = spawnPointContainer.position + new Vector3(x, 2.0f, z);

            GameObject go = new GameObject("SpawnPoint " + i);
            go.transform.position = pos;
            go.transform.parent = spawnPointContainer;

            spawnPoints.Add(go.transform);
        }

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoints[i].position, Quaternion.identity);
            enemy.GetComponent<PlayerInput>().enabled = false;
            enemy.GetComponent<RVP.FollowAI>().target = player.transform;
            enemy.GetComponent<TargetAI>().target = player.transform;
        }
    }
}
