using UnityEngine;
using UnityEngine.AI;

public class AI_Spawner : MonoBehaviour
{
    public GameObject aiPrefab; // The AI prefab to spawn
    public BoxCollider spawnArea; // BoxCollider defining the spawn area
//******************************************************************************************************************************************

    void Start()
    {
        spawnArea = GetComponent<BoxCollider>();
        if (spawnArea == null || aiPrefab == null)
        {
            Debug.LogError("Spawner requires a BoxCollider and AI prefab.");
            return;
        }
    }

//******************************************************************************************************************************************

    public void SpawnAI(int count,float oPrio,float pPrio)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = GetRandomPointInSpawnArea();

            // makes sure spawn position is on NavMesh
            if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                GameObject temp = null;
                temp = (GameObject)Instantiate(aiPrefab, hit.position, Quaternion.identity); //instantiate unit
                AmaiseAI tempAI = temp.GetComponent<AmaiseAI>();

                tempAI.objectivePriority = oPrio; //set objective and player priority
                tempAI.playerPriority = pPrio;
                Debug.Log($"AI spawned at: {hit.position}");
            }
            else
            {
                Debug.LogWarning($"Failed to find a NavMesh point near {spawnPosition}. Making another attempt.");
                i--;
            }
        }
    }
    // random spot generator
    private Vector3 GetRandomPointInSpawnArea()
    {
        // Get the bounds of the BoxCollider
        Bounds bounds = spawnArea.bounds;

        // Generate a random position within the bounds
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = bounds.center.y; // Use the collider's center height
        float z = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(x, y, z);
    }
    
        

}
