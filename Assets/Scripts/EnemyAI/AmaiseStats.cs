using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAmaiseStats", menuName = "Amaise/AmaiseStats")]
public class AmaiseStats : ScriptableObject
{
    public float moveSpeed;
    public int damage;
    public int health;
    public float attackCooldown;
    public float attackRange;
    public float detectionRange;
    public float playerPriority;
    public float objectivePriority;
}
