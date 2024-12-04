using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AmaiseAI : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // AudioSource to play sounds
    [SerializeField] private AudioClip deathSFX; // Audio for death
    public AmaiseStats stats; // Reference to the ScriptableObject
    private GameObject self;    // for use in self referencing
    [SerializeField] private NavMeshAgent agent;// navmesh

    [SerializeField] private Transform player;//player transform
    [SerializeField] private Transform objective;//objective transform
    public GameObject target;

    private Vector3 lastSetDestination;
    public float currHealth;
    private float lastAttackTime;
    private float attackRange;
    private float attackDamage;
    
    [SerializeField]private float detectionRange;
    private float updateTargetInterval = 2f; // Interval to update the target position
    private float lastDestinationUpdateTime; // Time of the last destination update
    private float lastTargetUpdateTime;
    public LayerMask obstructionMask;   // for use in raycasting

    public bool canSeePlayer;

    public int damageObjective = 0;//for tracking damage to objective
    public int damagePlayer = 0;//for tracking damage to player
    public float playerPriority = .6f; // player prio
    public float objectivePriority = .4f;//objective prio

    public bool objectiveDestroyed = false; // helps with logic if all obejctives are destroyed

    void Start()
    {
        //getting reference to self for later use
        self = this.GameObject();

        if (stats == null)//if the stats object isn't connected to the prefab
        {
            Debug.LogError("AmaiseStats is not assigned!");
            return;
        }

        // Initialize stats from ScriptableObject
        currHealth = stats.health;
        playerPriority = stats.playerPriority;
        objectivePriority = stats.objectivePriority;
        detectionRange = stats.detectionRange;

        // Setup NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = stats.moveSpeed;
        }

        // Find player and objective
        player = GameObject.FindWithTag("Player")?.transform;
        objective = GameObject.FindWithTag("Objective")?.transform;
        
        // initial path set and starting path update loop
        UpdateNavMeshDestination();
        lastDestinationUpdateTime = Time.time;
        lastTargetUpdateTime = Time.time;
    }

    void Update()
    {
        AdjustPriorities();
        if (currHealth <= 0 || agent == null) return;

        player = GameObject.FindWithTag("Player")?.transform;

        if (agent == null) return;
            
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            
        // Update the destination only every 2 seconds
        if (Time.time >= lastDestinationUpdateTime + updateTargetInterval)
        {
            UpdateNavMeshDestination();
            lastDestinationUpdateTime = Time.time;
        }

        // Perform actions based on visibility and distance
        if (distanceToPlayer < detectionRange)
        {
            playerPriority += .1f;
            if (distanceToPlayer <= attackRange)
            {
                
                if (CanSeePlayer())
                {
                    
                    agent.isStopped = true;
                    Debug.Log("AI stopped to attack the player.");
                    Attack(); // Perform the attack
                    // Resume movement after the attack cooldown
                    StartCoroutine(ResumeMovementAfterAttack(stats.attackCooldown));
                }

                
            }
            NormalizePriorities();
        }
        
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
            if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstructionMask))
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
        if (agent.isStopped) return;
        if(playerPriority>objectivePriority || CanSeePlayer())
        {
            MoveToPlayer();
            target = GameObject.FindWithTag("Player");
        }

        else
        {
            MoveToObjective();
            target = GameObject.FindWithTag("Objective");
        }
    }
    private void MoveToPlayer()
    {
        if (player != null && agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
            Debug.Log("Moving toward player.");
        }
    }

    private void MoveToObjective()
    {
        if (objective != null && agent.isOnNavMesh)
        {
            agent.SetDestination(objective.position);
            Debug.Log("Moving toward objective.");
        }
        else
        {
            Debug.Log("Objective destroyed. Switching priorities.");
            AdjustPrioritiesForObjectiveDestruction();
        }
    }

//******************************************************************************************************************************************
    private void Attack()
    {
        if (Time.time >= lastAttackTime + stats.attackCooldown && agent.enabled == true)
        {
            Debug.Log("Attacking!");

            // Stop movement while attacking
            agent.isStopped = true;

            // Perform attack logic (e.g., deal damage to player)
            if (player != null)
            {
                this.GameObject().GetComponent<AI_attack_handler>().Attack(target, attackDamage);
                agent.enabled = false;
            }
            lastAttackTime = Time.time;
        }
        else { Debug.Log("Reloading"); }
    }
    private IEnumerator ResumeMovementAfterAttack(float delay) // will delay resumption of movement
    {
        yield return new WaitForSeconds(delay);

        if (agent != null)
        {
            agent.isStopped = false; // Resume movement after the attack delay
            agent.enabled = true;
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
        if (currHealth <= stats.health / 2)
        {
            Debug.Log("Health is below half. Prioritizing player.");
            playerPriority += 0.1f;
        }
        if (CanSeePlayer())
        {
            Debug.Log("Player out of range. Prioritizing objective.");
            playerPriority += 0.1f;
        }
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
        if (currHealth <= 0)
        {
            audioSource?.PlayOneShot(deathSFX);
            agent.enabled = false;
            target = null;
            detectionRange = 0f;
        }
        else
        {
            Debug.Log("Taking" + damage+ " damage. Prioritizing player.");
            playerPriority += 0.1f;
            NormalizePriorities();
        }
    }
    public void handleDeath()
    {
        Destroy(this.gameObject);
    }
}