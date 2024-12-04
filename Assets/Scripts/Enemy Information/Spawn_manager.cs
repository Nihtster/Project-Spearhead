using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_manager : MonoBehaviour
{

    [SerializeField] GameObject[] spawnObject;
    [SerializeField] List<AI_Spawner> spawners;

    private int activeUnits = 0;
    private int unitsPerSpawner = 2;
    [SerializeField] private int spawnDelay = 15;

    // Start is called before the first frame update
    void Start()
    {
        compileSpawners();
        spawnWave(2, .5f,.5f);
        spawners = new List<AI_Spawner> ();
    }

    // Update is called once per frame
    
//******************************************************************************************************************************************
    private void compileSpawners()
    {
        foreach (GameObject spawner in spawnObject)
        {
            spawners.Add(spawner.GetComponent<AI_Spawner>());
        }
    }
    private void spawnWave(int numUnit,float oPrio,float pPrio)
    {
        foreach(AI_Spawner spawner in spawners)
        {
            spawner.SpawnAI(numUnit, oPrio, pPrio);
            activeUnits += numUnit;
        }
    }
    public IEnumerator waveSpawnDelayRoutine()
    {
        yield return new WaitForSeconds(spawnDelay);
        
        spawnWave(unitsPerSpawner, .8f,.2f);

        spawnDelay -= (int)(spawnDelay * .75);
        Debug.Log("AI unit wave spawned, " + spawnDelay+" seconds until next spawn");
        StopCoroutine(waveSpawnDelayRoutine());
     }
 //******************************************************************************************************************************************



}
