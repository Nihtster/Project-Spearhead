using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Spawner : MonoBehaviour
{
    public GameObject aiPrefab; // The AI prefab to spawn
    [SerializeField]private int numberOfUnits; // Approximate number of AI units to spawn
    public BoxCollider spawnArea; // BoxCollider defining the spawn area

    void Start()
    {
        if (numberOfUnits == 0) 
        {
            numberOfUnits = 5;
        }
        spawnArea = GetComponent<BoxCollider>();
        if (spawnArea == null || aiPrefab == null)
        {
            Debug.LogError("Spawner requires a BoxCollider and AI prefab.");
            return;
        }

        SpawnAI();
    }

    private void SpawnAI()
    {
        for (int i = 0; i < numberOfUnits; i++)
        {
            Vector3 spawnPosition = GetRandomPointInSpawnArea();

            // makes sure spawn position is on NavMesh
            if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                Instantiate(aiPrefab, hit.position, Quaternion.identity);
                Debug.Log($"AI spawned at: {hit.position}");
            }
            else
            {
                Debug.LogWarning($"Failed to find a NavMesh point near {spawnPosition}. Skipping spawn.");
            }
        }
    }
    // random spot generator(can produce overlapping models)
    private Vector3 GetRandomPointInSpawnArea()
    {
        // Get the bounds of the BoxCollider
        Bounds bounds = spawnArea.bounds;

        // Generate a random position within the bounds
        float x = Random.Range(bounds.min.x + 13, bounds.max.x+13);
        float y = bounds.center.y; // Use the collider's center height
        float z = Random.Range(bounds.min.z+13, bounds.max.z + 13);

        return new Vector3(x, y, z);
    }
}
