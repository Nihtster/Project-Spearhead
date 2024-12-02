using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryController : MonoBehaviour
{
    [SerializeField] private GameObject primaryWeapon;
    [SerializeField] private GameObject fx;

    [SerializeField] private int numMags;
    [SerializeField] private int magSize;
    [SerializeField] private float fireRate; // cooldown between shots (in seconds)
    [SerializeField] private float reloadTime; // in seconds
    [SerializeField] private float DMG;

    private void fire() {

    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            fire();
    }
}
