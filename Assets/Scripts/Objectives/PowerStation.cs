using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerStation : Objective
{
    [SerializeField] private Objective[] linked;

    protected void Death()
    {
        base.Death();
        foreach (Objective obj in linked)
        {
            obj.Disable();
        }
    }
}
