using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Objective : MonoBehaviour
{
    [SerializeField] protected AudioSource audioSource; // AudioSource to play sounds
    [SerializeField] private AudioClip dmgAlert;
    [SerializeField] private TMP_Text text_interface;
    [SerializeField] private Image bg_hp_indicator;
    [SerializeField] private Camera playerCam;
    [SerializeField] private string Name = "Base";
    [SerializeField] private float Health = 100;
    protected float curHealth;
    protected bool alerted = false;
    protected bool destroyed = false;
    protected bool disabled = false;

    protected void Death() { destroyed = true; Disable(); }
    public void Disable() { disabled = true; }

    protected void Start() { Debug.Log("Objective Initialized"); curHealth = Health; updateInterface(); }
    protected void Update() {
        if (playerCam != null)
        {
            Vector3 directionToCamera = playerCam.transform.position - transform.position;
            directionToCamera.y /= 4; // reduce vertical rotation
            transform.rotation = Quaternion.LookRotation(-directionToCamera); // it actually faces us
        }
    }

    protected void updateInterface()
    {
        // Determine player distance here and give 1st char or full name if close
        text_interface.text = Name[0] + "";
        bg_hp_indicator.color = Color.Lerp(new Color(1f, 0f, 0f), new Color(0f, 1f, 0f), Mathf.Clamp01((float)curHealth / Health));
    }

    public float dmg(float amt)
    {
        curHealth -= amt;
        updateInterface();
        if (curHealth <= 0) { bg_hp_indicator.color = new Color(0.33f, 0.33f, 0.33f); Death(); }
        if (!alerted) { StartCoroutine(AlertPlayerOnDMG()); }
        return curHealth;
    }

    protected IEnumerator AlertPlayerOnDMG()
    {
        alerted = true;
        audioSource.PlayOneShot(dmgAlert);
        yield return new WaitForSeconds(60);
        alerted = false;
    }

    public bool getDestroyed() 
    {
        return destroyed;
    }

    public bool getDisabled() 
    {
        return disabled;
    }
}
