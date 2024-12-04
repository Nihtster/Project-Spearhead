using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_manager : MonoBehaviour
{

    [SerializeField] GameObject[] spawnObject;
    [SerializeField] List<AI_Spawner> spawners;

    private float objectivePrioTotal = 6f;
    private float playerPrioTotal = 6f;
    private float objectiveDamage = 1f;
    private float playerDamage = 1f;
    private int activeUnits = 0;
    private int unitsPerSpawner = 2;
    private float newObjPrio = .4f;
    private float newPlayerPrio = .6f;
    private bool spawnersCompiled = false;

    // Start is called before the first frame update
    void Start()
    {
        compileSpawners();
        spawnWave(2, .4f,.6f);
        spawners = new List<AI_Spawner> ();
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnersCompiled)
        {

            AI_Spawner temp = null;
            foreach (AI_Spawner spawner in spawners)
            {
                temp = spawner;
                if (spawner.numUnits == 0)
                {
                    objectiveDamage += spawner.damageObjective;
                    playerDamage += spawner.damagePlayer;
                    spawner.damageObjective = 0;
                    spawner.damagePlayer = 0;
                    activeUnits -= unitsPerSpawner;
                    break;
                }
            }
            if(temp != null && temp.damagePlayer == 0)
            {
                spawners.Remove(temp);
            }
        }
        if(spawners.Count == 0)
        {
            float oPrio = calcObjectivePrio(unitsPerSpawner);
            float pPrio = calcPlayerPrio(unitsPerSpawner);
            float total = oPrio + pPrio;
            oPrio /= total;
            pPrio /= total;
            newObjPrio = oPrio;
            newPlayerPrio = pPrio;
            waveSpawnDelayRoutine();
        }

    }
//******************************************************************************************************************************************
    private void compileSpawners()
    {
        foreach (GameObject spawner in spawnObject)
        {
            spawners.Add(spawner.GetComponent<AI_Spawner>());
        }
        spawnersCompiled = true;
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
        yield return new WaitForSeconds(5);
        

        spawnWave(unitsPerSpawner, newObjPrio, newPlayerPrio);
        objectiveDamage = 0;
        playerDamage = 0;
        objectivePrioTotal = 0;
        playerPrioTotal = 0;
        Debug.Log("AI units spawned");
     }
 //******************************************************************************************************************************************
    private float calcObjectivePrio(int units)
    {
        return (objectivePrioTotal / (unitsPerSpawner * 6))*(objectiveDamage/(playerDamage+objectiveDamage));
    }
    private float calcPlayerPrio(int units)
    {
        return (playerPrioTotal/(unitsPerSpawner * 6))*(playerDamage / (playerDamage + objectiveDamage));
    }


}
