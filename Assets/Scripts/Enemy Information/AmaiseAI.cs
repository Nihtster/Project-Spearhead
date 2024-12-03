using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AmaiseAI : MonoBehaviour
{
    public AmaiseStats stats; // Reference to the ScriptableObject
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private Transform objective;

    private Vector3 lastSetDestination;
    private float currHealth;
    private float lastAttackTime;
    private float attackRange;
    [SerializeField]private float detectionRange;
    private float updateTargetInterval = 5f; // Interval to update the target position
    private float lastDestinationUpdateTime; // Time of the last destination update
    private float lastTargetUpdateTime;
    public LayerMask obstructionMask;

    private float playerPriority = 0.5f;
    private float objectivePriority = 0.5f;

    private bool objectiveDestroyed = false;

    void Start()
    {
        if (stats == null)
        {
            Debug.LogError("AmaiseStats is not assigned!");
            return;
        }

        // Initialize stats from ScriptableObject
        currHealth = stats.health;
        attackRange = stats.attackRange;
        detectionRange = 1750f; // Customize as needed

        // Setup NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = stats.moveSpeed;
        }

        // Find player and objective
        player = GameObject.FindWithTag("Player")?.transform;
        objective = GameObject.FindWithTag("Objective")?.transform;


        lastDestinationUpdateTime = Time.time;
        lastTargetUpdateTime = Time.time;
    }

    void Update()
    {
        if (agent == null) return;



        // Update priorities if necessary
        if (Time.time >= lastTargetUpdateTime + updateTargetInterval || HasPrioritySwitched())
        {
            UpdateTargetPriorities();
            lastTargetUpdateTime = Time.time;
        }

        // Update the destination only every 5 seconds
        if (Time.time >= lastDestinationUpdateTime + updateTargetInterval)
        {
            UpdateNavMeshDestination();
            lastDestinationUpdateTime = Time.time;
        }
        //debug log for distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Debug.Log($"Distance from AI to Player: {distanceToPlayer}");
        // Perform actions based on visibility and distance
        if (CanSeePlayer())
        {

            if (distanceToPlayer <= attackRange)
            {
                agent.isStopped = true;
                Debug.Log("AI stopped to attack the player.");

                // Perform the attack
                Attack();

                // Resume movement after the attack cooldown
                StartCoroutine(ResumeMovementAfterAttack(stats.attackCooldown));
            }
        }

    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstructionMask))
            {
                return true; // Player is visible
            }
        }
        return false;
    }
    private void UpdateNavMeshDestination()
    {
        if (CanSeePlayer() && !agent.isStopped)
        {
            MoveToPlayer();
        }
        else if (!agent.isStopped)
        {
            MoveToObjective();
        }
    }
    private void MoveToPlayer()
    {
        if (player != null && agent.isOnNavMesh)
        {
            agent.isStopped = false; // Resume movement if stopped
            agent.SetDestination(player.position);
            Debug.Log("Moving toward player.");
        }
    }

    private void MoveToObjective()
    {
        if (objective != null && agent.isOnNavMesh && !objectiveDestroyed)
        {
            agent.isStopped = false; // Resume movement if stopped
            agent.SetDestination(objective.position);
            Debug.Log("Moving toward objective.");
        }
        else
        {
            Debug.Log("Objective destroyed. Switching priorities.");
            AdjustPrioritiesForObjectiveDestruction();
        }
    }
// @TODO
    private void Attack()
    {
        if (Time.time >= lastAttackTime + stats.attackCooldown)
        {
            Debug.Log("Attacking!");

            // Stop movement while attacking
            agent.isStopped = true;

            // Perform attack logic (e.g., deal damage to player)
            if (player != null)
            {
                /*
                 * @TODO Need to make the target take damage(Component damage?)
                 */

                // Example: Apply damage to the player
                //player.GetComponent<PlayerHealth>()?.TakeDamage(stats.damage);
            }
            lastAttackTime = Time.time;
        }
    }
    private void UpdateTargetPriorities()
    {
        if (currHealth <= stats.health / 2)
        {
            Debug.Log("Health is below half. Prioritizing player.");
            playerPriority += 0.1f;
        }

        if (!CanSeePlayer())
        {
            Debug.Log("Player out of range. Prioritizing objective.");
            objectivePriority += 0.1f;
        }

        NormalizePriorities();
    }
    private bool HasPrioritySwitched()
    {
        float previousPlayerPriority = playerPriority;
        float previousObjectivePriority = objectivePriority;

        AdjustPriorities();

        return playerPriority != previousPlayerPriority || objectivePriority != previousObjectivePriority;
    }
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

        if (!CanSeePlayer())
        {
            Debug.Log("Player out of range. Prioritizing objective.");
            objectivePriority += 0.1f;
        }

        NormalizePriorities();
    }

    private void NormalizePriorities()
    {
        float total = playerPriority + objectivePriority;
        playerPriority /= total;
        objectivePriority /= total;
    }

    private IEnumerator ResumeMovementAfterAttack(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (agent != null)
        {
            agent.isStopped = false; // Resume movement after the attack delay
        }

        Debug.Log("Resuming movement.");
    }

    public void TakeDamage(int damage)
    {
        currHealth -= damage;

        if (currHealth <= 0)
        {
            Destroy(gameObject); // Destroy the AI when health reaches zero
        }
        else
        {
            Debug.Log("Taking damage. Prioritizing player.");
            playerPriority += 0.1f;
            NormalizePriorities();
        }
    }

    public void OnObjectiveDestroyed()
    {
        objectiveDestroyed = true;
        AdjustPrioritiesForObjectiveDestruction();
    }
}