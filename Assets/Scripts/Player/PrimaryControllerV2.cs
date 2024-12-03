using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryControllerV2 : MonoBehaviour
{
    [SerializeField] private GameObject primaryWeapon;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject fx;
    [SerializeField] private Camera cam;
    [SerializeField] private AudioClip fireSound; // Audio for firing
    [SerializeField] private AudioClip impactSound; // Audio for impact
    [SerializeField] private AudioSource audioSource; // AudioSource to play sounds

    [SerializeField] private int roundsLeft;
    [SerializeField] private int magSize;
    [SerializeField] private float fireRate; // cooldown between shots (in seconds)
    [SerializeField] private float reloadTime; // in seconds
    [SerializeField] private float projectileSpeed = 20f; // Projectile speed
    [SerializeField] private float fireDelay = 0.5f; // Delay before impact sound plays (in seconds)
    [SerializeField] private float hitMarkerDelay = 0.3f; // Delay before placing the hit marker (in seconds)

    private int mag;
    private int initialRounds;

    private bool firing; 
    private bool reloading;

    private void reload()
    {
        reloading = true;
        int diff = magSize - mag; // how much we are reloading
        int possible = roundsLeft < diff ? roundsLeft : diff; // how much we can reload from ammo
        mag += possible;
        roundsLeft -= possible;
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
        if (audioSource == null) 
        {
            audioSource = GetComponent<AudioSource>(); // Get the AudioSource component if not assigned
        }
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
        PlaySound(fireSound); // Play fire sound
        firing = true;
        StartCoroutine(fireRoutine());
    }

    private IEnumerator fireRoutine()
    {
        // Raycast from the position of the primaryWeapon
        Ray ray = new Ray(primaryWeapon.transform.position, primaryWeapon.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f); // Draw the ray for 1 second, with a red color and length of 100 units

        // Cast the ray forward and check if it hits anything
        if (Physics.Raycast(ray, out hit))
        {
            // Wait for the specified hit marker delay before placing the hit marker
            yield return new WaitForSeconds(hitMarkerDelay);
            
            // If the ray hits something, instantiate the visual effect (impact)
            Instantiate(fx, hit.point, Quaternion.LookRotation(hit.normal));

            // Play the impact sound
            PlaySound(impactSound);
        }

        // Always instantiate the projectile regardless of whether it hit or not
        GameObject firedProjectile = Instantiate(projectile, primaryWeapon.transform.position, Quaternion.identity);
        firedProjectile.GetComponent<Rigidbody>().velocity = primaryWeapon.transform.forward * projectileSpeed; // Move the projectile with adjustable speed

        --mag;
        if (mag <= 0) { reload(); }
        yield return new WaitForSeconds(fireRate); // Wait for fire rate cooldown
        firing = false; // End firing
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip); // Play the provided sound clip
        }
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
