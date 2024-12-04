using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerStation : Objective
{
    [SerializeField] private PlayerController Player;
    [SerializeField] private Objective[] poweredObjectives;

    protected void Death()
    {
        base.Death();
        foreach (Objective obj in poweredObjectives)
        {
            obj.Disable();
        }
        Player.Death();
    }
}
