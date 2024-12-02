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
    private Transform target;

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

        GameObject player = GameObject.FindWithTag("Player");
        target = player.transform;
    }

    void Update()
    {
        if (target != null && agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);

            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget <= attackRange)
            {
                //AttemptAttack();
                //SelfDestruct();
            }
        }
        Debug.Log("Current health is " + currHealth);
    }

    private void SelfDestruct()
    {
        TakeDamage(currHealth);
    }

    private void AttemptAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log($"Amaise attacks the player for {damage} damage!");

            lastAttackTime = Time.time; // Reset cooldown
        }
    }


    public void TakeDamage(int damage)
    {
        currHealth -= damage;

        if(currHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
