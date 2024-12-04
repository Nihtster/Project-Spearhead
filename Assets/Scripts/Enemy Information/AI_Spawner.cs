using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Spawner : MonoBehaviour
{
    public GameObject aiPrefab; // The AI prefab to spawn
    [SerializeField]private int numberOfUnitsTotal; // number of units spawned in total
    public BoxCollider spawnArea; // BoxCollider defining the spawn area
    private List<GameObject> unitsSpawned; // holds units currently spawned from this spawner
    public int numUnits = 0;
    public int damageObjective = 0;//for tracking damage to objective
    public int damagePlayer = 0;//for tracking damage to player
//******************************************************************************************************************************************

    void Start()
    {
        spawnArea = GetComponent<BoxCollider>();
        if (spawnArea == null || aiPrefab == null)
        {
            Debug.LogError("Spawner requires a BoxCollider and AI prefab.");
            return;
        }
        unitsSpawned = new List<GameObject>();
    }

    private void Update()
    {
        if (unitsSpawned != null)
        {
            foreach (GameObject go in unitsSpawned)
            {
                if (go.GetComponent<AmaiseAI>().currHealth <= 0)
                {
                    AmaiseAI tempAI = go.GetComponent<AmaiseAI>();
                    damageObjective += tempAI.damageObjective;
                    damagePlayer += tempAI.damagePlayer;
                    unitsSpawned.Remove(go);
                    HandleDeath(go);
                }
            }
        }
    }
//******************************************************************************************************************************************
    private void HandleDeath(GameObject go)
    {
        if (go != null)
        {
            go.GetComponent<AmaiseAI>().handleDeath();
        }
        Debug.Log("unit Despawned");
    }

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
                unitsSpawned.Add(temp);     //add unit to internal list

                tempAI.objectivePriority = oPrio; //set objective and playe priority
                tempAI.playerPriority = pPrio;
                Debug.Log($"AI spawned at: {hit.position}");
                numUnits++; // increase currently spawned units
                numberOfUnitsTotal++;// increase total spawned units
            }
            else
            {
                Debug.LogWarning($"Failed to find a NavMesh point near {spawnPosition}. Skipping spawn.");
            }
        }
    }
    // random spot generator
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
