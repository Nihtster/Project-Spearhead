using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class AI_attack_handler : MonoBehaviour
{

    public float Attack(GameObject target, float damage)
    {
        Debug.Log("Attacking!");

        if(target.CompareTag("Objective"))  //attacking objective
        {
            target.GetComponent<Objective>().dmg(damage);
            return damage;
        }else if (target.CompareTag("Player"))
        {
            target.GetComponent<PlayerController>().dmg(damage);
            return damage;
        }
        return 0;

    }
}
