using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryController : MonoBehaviour
{
    [SerializeField] private GameObject left, right;
    [SerializeField] private GameObject fxL, fxR;

    [SerializeField] private int numMags;
    [SerializeField] private int magSize;
    [SerializeField] private float fireRate; // cooldown between shots (in seconds)
    [SerializeField] private float reloadTime; // in seconds
    [SerializeField] private float DMG;

    private void fire() {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            fire();
    }
}
