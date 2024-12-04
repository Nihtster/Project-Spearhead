using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AI_patrolling : MonoBehaviour
{
    private float patrolRadius = 20f; // Radius within which the AI will patrol
    private float patrolWaitTime = 3f; // Time to wait at each patrol point
    private Vector3 currentPatrolPoint; // Current patrol destination
    private bool isPatrolling = false;
    private AmaiseAI unitAI;
    private NavMeshAgent agent;

    private void Start()
    {
        unitAI = this.GameObject().GetComponent<AmaiseAI>();
        agent = this.GameObject().GetComponent<NavMeshAgent>();
    }
    private void Patrol()
    {
        if (!isPatrolling)
        {
            StartCoroutine(PatrolRoutine());
        }
    }
    private IEnumerator PatrolRoutine()
    {
        isPatrolling = true;
        while (unitAI.target == null || !unitAI.canSeePlayer && unitAI.objectiveDestroyed)
        {
            // Find a random patrol point within the radius
            Vector3 patrolPoint = GetRandomPatrolPoint();

            // Move to the patrol point
            if (NavMesh.SamplePosition(patrolPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                Debug.Log($"Patrolling to point: {hit.position}");
                currentPatrolPoint = hit.position;
            }

            // Wait until the AI reaches the patrol point
            while (agent.remainingDistance > agent.stoppingDistance)
            {
                yield return new WaitForEndOfFrame();
            }

            // Wait for a while at the patrol point
            Debug.Log("Waiting at patrol point.");
            yield return new WaitForSeconds(patrolWaitTime);
        }

        isPatrolling = false;
    }

    private Vector3 GetRandomPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position; // Offset by the AI's position
        randomDirection.y = transform.position.y; // Keep the AI on the same height level
        return randomDirection;
    }

    void Update()
    {
        if (agent == null) return;

        // Patrol if no player is nearby and the objective is destroyed
        if (!unitAI.canSeePlayer && unitAI.objectiveDestroyed)
        {
            Patrol();
            return;
        }
    }
}
