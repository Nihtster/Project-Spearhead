using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resupply : Objective
{
    [SerializeField] private AudioClip resupplySFX;

    void Start()
    {
        
    }

    void Update()
    {
        base.Update();
        if(disabled) { return; }
        // if player is nearby, repair, resupply, play sfx and stuff
    }

}
