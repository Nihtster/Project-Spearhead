using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AmaiseAI : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;       // AudioSource to play sounds
    [SerializeField] private AudioClip deathSFX;            // Audio for death
    [SerializeField] private AmaiseStats stats;                               // Reference to the ScriptableObject
    [SerializeField] private NavMeshAgent agent;            // navmesh
    [SerializeField] private Transform player;              // player transform
    [SerializeField] private Transform objective;           // objective transform
    [SerializeField] public GameObject target;

    [SerializeField] private Vector3 lastSetDestination;
    [SerializeField] public float currHealth;
    [SerializeField] private float lastAttackTime;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackDamage;
    [SerializeField] private float detectionRange;
    [SerializeField] public LayerMask obstructionMask;                       // for use in raycasting
    [SerializeField] public float playerPriority = .6f;                      // player prio
    [SerializeField] public float objectivePriority = .4f;                   // objective prio
    [SerializeField] public bool objectiveDestroyed = false;                 // helps with logic if all objectives are destroyed
    [SerializeField] public bool firing;
    [SerializeField] public bool canSeePlayer;


    void Start()
    {
        if (stats == null)                                  //if the stats object isn't connected to the prefab
        {
            Debug.LogError("AmaiseStats is not assigned!");
            return;
        }
                                                            // Initialize stats from ScriptableObject
        currHealth = stats.health;
        playerPriority = stats.playerPriority;
        objectivePriority = stats.objectivePriority;
        detectionRange = stats.detectionRange;
        attackRange = stats.attackRange;
                                                            // Setup NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = stats.moveSpeed;
        }
                                                            // Find player and objective
        player = GameObject.FindWithTag("Player")?.transform;
        objective = GameObject.FindWithTag("Objective")?.transform;
        target = player.gameObject;
                                                            // initial path set and starting path update loop
        UpdateNavMeshDestination();
    }

    void Update()
    {
        player = GameObject.FindWithTag("Player")?.transform;
                                                             // adjusting priorities(normalizing)
        if (objectiveDestroyed)  AdjustPrioritiesForObjectiveDestruction(); 
        else AdjustPriorities();

        if (currHealth <= 0 || agent == null) return;
            
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float distanceToObjective = Vector3.Distance(transform.position, objective.position);

        UpdateNavMeshDestination();
                                                            // Perform actions based on visibility and distance
        NormalizePriorities();
                                                            // Perform the attack
        if (distanceToPlayer < attackRange) Attack(player.gameObject);
        else if (distanceToObjective < attackRange) Attack(objective.gameObject);
    }
//******************************************************************************************************************************************
    private bool CanSeePlayer()
    {
        if (player == null) return false;
        
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
                                                            //debug log for distance to player
        //Debug.Log($"Distance from AI to Player: {distanceToPlayer}");

        if (distanceToPlayer <= detectionRange)
        {
            if (!Physics.Raycast(transform.position, directionToPlayer, detectionRange, obstructionMask))
            {
                canSeePlayer = true;
                return true; // Player is visible
                
            }
        }
        canSeePlayer = false;
        return false;
    }
//******************************************************************************************************************************************

                                                            // sets destination, changes route if near player.
    private void UpdateNavMeshDestination()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (agent.isStopped) return;
        if(CanSeePlayer() || playerPriority >= objectivePriority)
        { 
            MoveToPlayer();
        }
        else
        {
            MoveToObjective();
        }
    }
    private void MoveToPlayer()
    {
        if (player != null && agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);

        }
    }
    private void MoveToObjective()
    {
        if (firing) return;
        if (objective == null) objective = GameObject.FindWithTag("Objective").transform; // if original objective is destroyed
            
        if (objective != null && agent.isOnNavMesh)
        {
            agent.SetDestination(objective.position);
        }
        else
        {
            Debug.Log("Objective destroyed. Switching priorities.");
            AdjustPrioritiesForObjectiveDestruction();
        }
    }

//******************************************************************************************************************************************
    private void Attack(GameObject t)
    {
        
                                                               // if already firing, will not fire again
        if (firing || t == null) 
        {
            return;
        }
        target = t;
        Debug.Log(target.tag);
        agent.isStopped = true;                                 // Stop movement while attacking
        firing = true;
        StartCoroutine(attackCoroutine(target, (float)stats.damage));  // Perform attack logic (e.g., deal damage to player)
    }
    public IEnumerator attackCoroutine(GameObject t, float damage)
    {
        if (t.CompareTag("Objective"))          // attacking objective
        {
            
            t.GetComponent<Objective>().dmg(damage);
            Debug.Log("Attacking Objective - successful");
        }
        else if (t.name.Contains("Player"))      // attacking player
        {
            PlayerController pl = t.GetComponent<PlayerController>();
            if (pl == null) Debug.Log("PC null");
            t.GetComponent<PlayerController>().dmg(damage);
            Debug.Log("Attacking Player - successful");
        }
        yield return new WaitForSeconds(stats.attackCooldown/12);


    }
    private IEnumerator ResumeMovementAfterAttack(float delay) // will delay resumption of movement
    {
        yield return new WaitForSeconds(delay/12);
        if (agent != null)
        {
            agent.isStopped = false;                        // Resume movement after the attack delay
            firing = false;
        }
        Debug.Log("Resuming movement.");
    }

//******************************************************************************************************************************************
   
    private void AdjustPrioritiesForObjectiveDestruction()
    {
        Debug.Log("Objective destroyed. Prioritizing player.");
        objectivePriority = 0.0f;
        playerPriority = 1.0f;
        NormalizePriorities();
    }

    private void AdjustPriorities()
    {                                                                         
        if (!CanSeePlayer())objectivePriority += 0.1f;
        else playerPriority += .1f;
        NormalizePriorities();
    }

    private void NormalizePriorities()
    {
        float total = playerPriority + objectivePriority;
        playerPriority /= total;
        objectivePriority /= total;
    }

    //******************************************************************************************************************************************
    public void dmg(int damage)
    {
        currHealth -= damage;
        if (currHealth <= 0) handleDeath();
        else
        {
            Debug.Log("Taking" + damage+ " damage.");
            AdjustPriorities();
        }
    }
    public void handleDeath()
    {
        audioSource?.PlayOneShot(deathSFX);
        target = null;
        detectionRange = 0f;
        Destroy(gameObject);
    }

}