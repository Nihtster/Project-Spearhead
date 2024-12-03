using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PrimaryController : MonoBehaviour
{
    [SerializeField] private TMP_Text statusInterface;
    [SerializeField] private TMP_Text ammoInterface;
    [SerializeField] private GameObject primaryWeapon;
    [SerializeField] private GameObject fx;
    [SerializeField] private Camera cam;
    [SerializeField] private AudioClip reloadSFX;
    [SerializeField] private AudioClip fireSFX; // Audio for firing
    [SerializeField] private AudioClip impactSFX; // Audio for impact
    [SerializeField] private AudioSource audioSource; // AudioSource to play sounds

    [SerializeField] private int roundsLeft = 30;
    [SerializeField] private int magSize = 5;
    [SerializeField] private float fireRate = 2.5f; // cooldown between shots (in seconds)
    [SerializeField] private float reloadTime = 5f; // in seconds
    [SerializeField] private float projectileSpeed = 20f; // Projectile speed
    [SerializeField] private float fireDelay = 0.5f; // Delay before impact sound plays (in seconds)
    [SerializeField] private float hitMarkerDelay = 0.3f; // Delay before placing the hit marker (in seconds)

    private int mag;
    private int initialRounds;

    private bool firing;
    private bool reloading;

    private void updateText()
    {
        ammoInterface.text = "" +
        mag + " / " + magSize + "\n" +
        "mag: " + roundsLeft;
    }

    private void reload()
    {
        reloading = true;
        StartCoroutine(StatusCoroutine());
        int diff = magSize - mag; // how much we are reloading
        int possible = roundsLeft < diff ? roundsLeft : diff; // how much we can reload from ammo
        StartCoroutine(ReloadCoroutine(possible));
    }

    private IEnumerator StatusCoroutine()
    {
        int idx = 0;
        while (reloading)
        {
            statusInterface.text = idx <= 0 ? "Reloading" : statusInterface.text + ".";
            ++idx;
            idx %= 4;
            yield return new WaitForSeconds(reloadTime / 12);
        }
        statusInterface.text = "";
    }

    private IEnumerator ReloadCoroutine(int amt)
    {
        PlaySound(reloadSFX);
        yield return new WaitForSeconds(reloadTime);
        PlaySound(reloadSFX);
        mag += amt;
        roundsLeft -= amt;
        updateText();
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
        updateText();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            fire();
        }
        if (Input.GetKeyDown(KeyCode.R) && !reloading && !firing && mag != 0 && mag != magSize)
        {
            reload();
        }
    }

    private void fire()
    {
        if (firing || reloading || mag <= 0) { return; } // Check if there's ammo
        firing = true;
        PlaySound(fireSFX); // Play fire sound
        StartCoroutine(fireRoutine());
    }

    private IEnumerator fireRoutine()
    {
        yield return new WaitForSeconds(fireDelay);
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
            PlaySound(impactSFX);
        }

        --mag;
        updateText();
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

    public void resupply()
    {
        roundsLeft = initialRounds;
        mag = magSize;
        updateText();
    }
}
