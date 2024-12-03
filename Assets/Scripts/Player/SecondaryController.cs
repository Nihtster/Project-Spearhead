using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SecondaryController : MonoBehaviour
{
    [SerializeField] private TMP_Text statusInterface;
    [SerializeField] private TMP_Text ammoInterface;
    [SerializeField] private GameObject secondaryWeaponL;
    [SerializeField] private GameObject secondaryWeaponR;
    [SerializeField] private GameObject fx;
    [SerializeField] private Camera cam;
    [SerializeField] private AudioClip reloadSFX;
    [SerializeField] private AudioClip fireSFX; // Audio for firing
    [SerializeField] private AudioClip impactSFX; // Audio for impact
    [SerializeField] private AudioSource audioSourceL; // AudioSource to play sounds
    [SerializeField] private AudioSource audioSourceR; // AudioSource to play sounds

    [SerializeField] private int roundsLeft = 120;
    [SerializeField] private int magSize = 12;
    [SerializeField] private float fireRate = 0.4f; // cooldown between shots (in seconds)
    [SerializeField] private float reloadTime = 3f; // in seconds
    [SerializeField] private float projectileSpeed = 100f; // Projectile speed

    private int mag;
    private int initialRounds;

    private bool firing;
    private bool reloading;
    private bool usedLeft;

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
        updateText();
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
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
        // Always instantiate the projectile regardless of whether it hit or not
        Vector3 calcPos = usedLeft ? secondaryWeaponL.transform.position : secondaryWeaponR.transform.position;
        Vector3 calcFwd = usedLeft ? secondaryWeaponL.transform.forward : secondaryWeaponR.transform.forward;
        // Raycast from the position of the secondaryWeapon
        Ray ray = new Ray(calcPos, calcFwd);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f); // Draw the ray for 1 second, with a red color and length of 100 units

        // Cast the ray forward and check if it hits anything
        if (Physics.Raycast(ray, out hit))
        {
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
        usedLeft = !usedLeft;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            if (usedLeft)
            {
                audioSourceL.PlayOneShot(clip);
            }
            else
            {
                audioSourceR.PlayOneShot(clip);
            }
        }
    }

    public void resupply()
    {
        roundsLeft = initialRounds;
        mag = magSize;
        updateText();
    }
}
