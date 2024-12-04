using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class AI_attack_handler : MonoBehaviour
{

    public float Attack(GameObject target, float damage)
    {
        if(target.CompareTag("Objective"))  //attacking objective
        {
            Debug.Log("Attacking Player!");
            target.GetComponent<Objective>().dmg(damage);
            return damage;
        }else if (target.CompareTag("Player"))
        {
            Debug.Log("Attacking Objective!");
            target.GetComponent<PlayerController>().dmg(damage);
            return damage;
        }
        return 0;

    }
}
