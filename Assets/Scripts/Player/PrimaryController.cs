using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryController : MonoBehaviour
{
    [SerializeField] private GameObject primaryWeapon;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject fx;
    [SerializeField] private Camera cam;

    [SerializeField] private int roundsLeft;
    [SerializeField] private int magSize;
    [SerializeField] private float fireRate; // cooldown between shots (in seconds)
    [SerializeField] private float reloadTime; // in seconds

    private int mag;
    private int initialRounds;

    private bool firing;
    private bool reloading;

    private void reload()
    {
        reloading = true;
        // TO-DO: reload sfx here
        int diff = magSize - mag; // how much we are reloading
        int possible = roundsLeft < diff ? roundsLeft : diff; // how much we can reload from ammo
        mag += possible;
        roundsLeft -= possible;
        // TO-DO: update player hud here
        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine() 
    {
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
    }

    void Start()
    {
        mag = magSize;
        initialRounds = roundsLeft;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            fire();
        }
    }
    
    private void fire()
    {
        if (firing || reloading || mag <= 0) { return; } // Check if there's ammo
        // TO-DO: fire sfx here
        firing = true;
        StartCoroutine(fireRoutine());
    }

    private IEnumerator fireRoutine()
    {
        GameObject firedProjectile = Instantiate(projectile, primaryWeapon.transform.position, Quaternion.identity);
        Vector3 projectileDirection = cam.transform.forward;
        // TO-DO: integrate with projectile script???
        --mag;
        if (mag <= 0) { reload(); }
        yield return new WaitForSeconds(fireRate);
        firing = false; // End firing
    }

    public int getRoundsLeft() { return roundsLeft; }
    public int getMagSize() { return magSize; }
    public int getMag() { return mag; }

    public void resupply()
    {
        roundsLeft = initialRounds;
        mag = magSize;
    }
}
