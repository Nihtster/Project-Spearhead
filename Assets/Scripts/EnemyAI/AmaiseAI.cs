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
    [SerializeField] private GameObject playerMain;              // player main object
    [SerializeField] private Transform player;              // player transform
    [SerializeField] private GameObject objective;           // objective transform
    [SerializeField] public GameObject target;

    [SerializeField] private Vector3 lastSetDestination;
    [SerializeField] public float currHealth;
    [SerializeField] private float lastAttackTime;
    [SerializeField] private float attackRange;
    [SerializeField] private float detectionRange;
    [SerializeField] public LayerMask obstructionMask;                       // for use in raycasting
    [SerializeField] public float playerPriority = .5f;                      // player prio
    [SerializeField] public float objectivePriority = .5f;                   // objective prio
    [SerializeField] public bool firing;
    [SerializeField] public bool canSeePlayer;
    private float destinationUpdateInterval = 0.5f; // Update every 0.5 seconds
    private float lastDestinationUpdateTime;

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
        playerMain = GameObject.FindWithTag("Player");              // player transform
        player = playerMain.GetComponent<PlayerController>().modelTransform;
        objective = GameObject.FindWithTag("Objective");           // objective transform
                                                            // initial path set and starting path update loop
        UpdateNavMeshDestination();
    }

    void Update()
    {
        
        if (currHealth <= 0 || agent == null) return;
        
        CanSeePlayer();
        
        if (Time.time >= lastDestinationUpdateTime + destinationUpdateInterval)
        {
            UpdateNavMeshDestination();
            lastDestinationUpdateTime = Time.time;
        }


        // Perform actions based on visibility and distance
        float distanceToPlayer = player != null ? Vector3.Distance(transform.position, player.transform.position) : Mathf.Infinity;
        if (canSeePlayer && distanceToPlayer <= attackRange)
        {
            Attack(playerMain.gameObject);
        }
        else if (objective != null && Vector3.Distance(transform.position, objective.transform.position) <= attackRange)
        {
            Attack(objective.gameObject);
        }
    }
//******************************************************************************************************************************************
    private bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        Debug.DrawRay(transform.position, directionToPlayer * detectionRange, Color.red);

        if(distanceToPlayer <= detectionRange && !Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstructionMask))
        {
            if (!canSeePlayer) // Visibility changed
            {
                canSeePlayer = true;
                Debug.Log("Player is now visible.");
            }
            playerPriority += .05f;
            return true;
        }

        if (canSeePlayer) // Visibility changed
        {
            canSeePlayer = false;
            Debug.Log("Player is no longer visible.");
        }
        return false;
    }
//******************************************************************************************************************************************
                                                            // sets destination, changes route if near player.
    private void UpdateNavMeshDestination()
    {
        float distanceToPlayer = player != null ? Vector3.Distance(transform.position, player.transform.position) : Mathf.Infinity;
        if (distanceToPlayer <= detectionRange || canSeePlayer)
        { 
            MoveToPlayer();
        }
        else if(objective !=null && !canSeePlayer)
        {
            MoveToObjective();
        }

    }
    private void MoveToPlayer()
    {
        
            agent.isStopped = false;
            Debug.Log(agent.SetDestination(player.transform.position)+"Moving toward player.");       
    }
    private void MoveToObjective()
    {
        
            agent.isStopped = false;
            Debug.Log(agent.SetDestination(objective.transform.position)+"Moving toward objective."); 
    }

//******************************************************************************************************************************************
    private void Attack(GameObject t)
    {
        if (Vector3.Distance(transform.position, player.position) > attackRange) return;
        // if already firing, will not fire again
        if (firing || t == null) 
        {
            return;
        }
        
        Debug.Log(t.tag);
        agent.isStopped = true;                                 // Stop movement while attacking
        firing = true;
        StartCoroutine(attackCoroutine(t, (float)stats.damage));  // Perform attack logic (e.g., deal damage to player)
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
        yield return new WaitForSeconds(stats.attackCooldown/2);
        yield return ResumeMovementAfterAttack(2);
    }
    private IEnumerator ResumeMovementAfterAttack(float delay) // will delay resumption of movement
    { 
        Debug.Log("Resuming movement.");
        agent.isStopped = false;                        // Resume movement after the attack delay
        firing = false;
        yield return new WaitForSeconds(delay);
    }

//******************************************************************************************************************************************

    private void AdjustPriorities()
    {                                                                         
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