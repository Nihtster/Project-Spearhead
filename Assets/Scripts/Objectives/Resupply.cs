using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resupply : Objective
{
    [SerializeField] private AudioClip resupplySFX;
    [SerializeField] private float radius = 3f;
    [SerializeField] private float cooldown = 120f;
    private bool resupplied = false;

    void Start()
    {
        base.Start();
        StartCoroutine(resupplyCheck());
    }

    private IEnumerator resupplyCheck()
    {
        while(!disabled) {
            yield return new WaitForSeconds(5f);
            if(resupplied) { continue; }
            Collider[] possiblePlayers = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider collider in possiblePlayers)
            {
                if (collider.CompareTag("Player_model"))
                {
                    GameObject plyObj = collider.transform.parent.parent.gameObject;
                    audioSource.PlayOneShot(resupplySFX);
                    resupplied = true;
                    PlayerController plyController = plyObj.GetComponent<PlayerController>();
                    plyController.restore();
                    PrimaryController primaryWeapon = plyObj.GetComponent<PrimaryController>();
                    primaryWeapon.resupply();
                    SecondaryController secondaryWeapon = plyObj.GetComponent<SecondaryController>();
                    secondaryWeapon.resupply();
                    StartCoroutine(resupplyDelay());
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private IEnumerator resupplyDelay()
    {
        yield return new WaitForSeconds(cooldown);
        resupplied = false;
    }

}
