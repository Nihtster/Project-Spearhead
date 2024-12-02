using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AmaiseAI : MonoBehaviour
{
    public AmaiseStats stats; // Reference to the ScriptableObject
    private NavMeshAgent agent;
    private Transform player;
    private Transform objective;
    private float currHealth;
    private float lastAttackTime;
    private float attackRange;
    // priority
    private float playerPriority = 0.5f;
    private float objectivePriority = 0.5f;
    // detection
    public float detectionRange = 50f;
    public LayerMask obstructionMask;
    private DecisionNode rootNode;

    void Start()
    {
        if (stats == null)
        {
            Debug.LogError("AmaiseStats is not assigned!");
            return;
        }
        currHealth = stats.health;
        attackRange = stats.attackRange;
        // setup NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = stats.moveSpeed;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
            }
        }
        // find player and objective
        player = GameObject.FindWithTag("Player")?.transform;
        objective = GameObject.FindWithTag("Objective")?.transform;

        // build the decision tree
        BuildDecisionTree();
    }

    void Update()
    {
        if (rootNode != null)
        {
            rootNode.Evaluate();
        }
    }
    private void BuildDecisionTree()
    {
        // Action Nodes
        ActionNode attackNode = new ActionNode(Attack);
        ActionNode moveToPlayerNode = new ActionNode(MoveToPlayer);
        ActionNode moveToObjectiveNode = new ActionNode(MoveToObjective);

        // Condition Nodes
        ConditionNode inRangeNode = new ConditionNode(IsInRange, attackNode, moveToPlayerNode);
        ConditionNode shouldAttackPlayerNode = new ConditionNode(ShouldAttackPlayer, inRangeNode, moveToObjectiveNode);

        // Root Node
        rootNode = shouldAttackPlayerNode;
    }

    // Decision Conditions
    private bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstructionMask))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsInRange()
    {
        if (player == null) return false;
        float distanceToTarget = Vector3.Distance(transform.position, player.position);
        return distanceToTarget <= attackRange;
    }

    private bool ShouldAttackPlayer()
    {
        return playerPriority > objectivePriority && CanSeePlayer();
    }

    // Actions
    private void MoveToPlayer()
    {
        if (player != null && agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
    }

    private void MoveToObjective()
    {
        if (objective != null && agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(objective.position);
        }
    }

    private void Attack()
    {
        if (Time.time >= lastAttackTime + stats.attackCooldown)
        {
            Debug.Log("Attacking!");

            if (playerPriority > objectivePriority && player != null)
            {
                //player.GetComponent<PlayerHealth>()?.TakeDamage(stats.damage);
            }
            else if (objective != null)
            {
                //objective.GetComponent<ObjectiveHealth>()?.TakeDamage(stats.damage);
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