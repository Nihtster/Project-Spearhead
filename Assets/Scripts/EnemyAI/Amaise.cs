using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class amaise : MonoBehaviour
{
    private int currHealth;
    private float moveSpeed;
    private int damage;
    private int health;
    public float attackRange = 50f;
    private float attackCooldown;
    private float lastAttackTime;

    private NavMeshAgent agent;
    private Transform player;
    private Transform objective;

    // Priority System
    private float playerPriority = 0.5f;
    private float objectivePriority = 0.5f;

    // Detection Parameters
    public float detectionRange = 50f;
    public LayerMask obstructionMask;

    void Start()
    {
        currHealth = 100;
        moveSpeed = 200;
        damage = 5;
        attackCooldown = 5f;

        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
            {
                transform.position = hit.position;  
            }
        }

        // Find player and objective
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null) player = playerObject.transform;

        GameObject objectiveObject = GameObject.FindWithTag("Objective");
        if (objectiveObject != null) objective = objectiveObject.transform;
    }

    void Update()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        // Decide target based on priority
        Transform target = DecideTarget();
        if (target != null)
        {
            agent.SetDestination(target.position);

            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget <= attackRange)
            {
                AttemptAttack();
            }
        }
        Debug.Log("Current health is " + currHealth);
    }
    private Transform DecideTarget()
    {
        if (player != null && CanSeePlayer())
        {
            return playerPriority > objectivePriority ? player : objective;
        }
        return objective; // Default to objective if player is not visible
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


    private void SelfDestruct()
    {
        TakeDamage(currHealth);
    }

    private void AttemptAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log($"Amaise attacks the target for {damage} damage!");
            if (playerPriority > objectivePriority && player != null)
            {
                // damage player
                //player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            }
            else if (objective != null)
            {
                // damage objective
                //objective.GetComponent<ObjectiveHealth>()?.TakeDamage(damage);
            }

            lastAttackTime = Time.time;
        }
    }


    public void TakeDamage(int damage)
    {
        currHealth -= damage;

        if (currHealth <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            // adjust priorities parameters if damaged by player
            AdjustPrioritiesOnDamage();
        }
    }

    private void AdjustPrioritiesOnDamage()
    {
        playerPriority += 0.1f;
        NormalizePriorities();
    }

    private void NormalizePriorities()
    {
        float total = playerPriority + objectivePriority;
        playerPriority /= total;
        objectivePriority /= total;
    }
}
