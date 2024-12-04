using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_manager : MonoBehaviour
{

    [SerializeField] GameObject[] spawnObject;
    [SerializeField] List<AI_Spawner> spawners;

    private int activeUnits = 0;
    private int unitsPerSpawner = 2;
    [SerializeField] private float spawnDelay = 60.0f;

    // Start is called before the first frame update
    void Start()
    {
        compileSpawners();
        StartCoroutine(waveSpawnRoutine());
        spawners = new List<AI_Spawner> ();
    }

    // Update is called once per frame
    
//******************************************************************************************************************************************
    private void compileSpawners()
    {
        if (spawnObject.Length == 0)
        {
            Debug.LogError("No spawn objects assigned.");
            return;
        }
        foreach (GameObject spawner in spawnObject)
        {
            spawners.Add(spawner.GetComponent<AI_Spawner>());
        }
    }
    private void spawnWave(int numUnit,float oPrio,float pPrio)
    {
        foreach (GameObject spawner in spawnObject)
        {
            Debug.Log("reaches spawn call");
            spawner.GetComponent<AI_Spawner>().SpawnAI(numUnit, oPrio, pPrio);
        }
        Debug.Log($"Wave spawned: {numUnit * spawners.Count} units.");

    }
    public IEnumerator waveSpawnRoutine()
    {
        while (true) // Infinite loop to continuously spawn waves
        {
            spawnWave(unitsPerSpawner, 0.8f, 0.2f);
            yield return new WaitForSeconds(spawnDelay);

            // Optional: Adjust spawn delay for subsequent waves
            spawnDelay = Mathf.Max(20.0f, spawnDelay * 0.85f); // Ensure a minimum spawn delay of 10 seconds
            Debug.Log($"Next wave in {spawnDelay} seconds.");
        }
    }
 //******************************************************************************************************************************************



}