using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public Transform modelTransform;
    [SerializeField] private PrimaryController primaryWeapon;
    [SerializeField] private SecondaryController secondaryWeapon;
    [SerializeField] private PlayerMovement movementController;
    [SerializeField] private Canvas loss_interface;
    [SerializeField] private TMP_Text health_interface;
    [SerializeField] private Image hurt_overlay;
    [SerializeField] private float health = 100f;
    private float curHealth;
    private int interval = 2; // health / interval = # of health bars
    private bool alive = true;

    public void Death()
    {
        alive = false;
        primaryWeapon.Disable();
        secondaryWeapon.Disable();
        movementController.Disable();
        loss_interface.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Start() { curHealth = health; updateInterface(); }//StartCoroutine(TestHealthInterface()); }

    private IEnumerator TestHealthInterface(){
        yield return new WaitForSeconds(3);
        dmg(100);
    }

    private void updateInterface()
    {
        // Determine color based off current health.
        string interface_text = "<color=#"+ColorUtility.ToHtmlStringRGB(Color.Lerp(new Color(1f, 0f, 0f), new Color(0f, 1f, 0f), Mathf.Clamp01((float)curHealth / health)))+">";
        for (int i = 0; i < curHealth / interval; i++)
        {
            interface_text += "i";
        }
        interface_text += "</color><color=grey>";
        for (int i = 0; i < (health - curHealth) / interval; i++)
        {
            interface_text += "i";
        }
        interface_text += "</color>";
        health_interface.text = interface_text;
    }

    private IEnumerator hurtOverlay(float dmg)
    {
        float opacity = (dmg / (health / 4)); // 25% of health is SEVERE dmg
        float fadeDuration = (dmg / health) * 2f;
        float elapsed = 0f;

        while(elapsed < fadeDuration){
            elapsed += Time.deltaTime;
            float newOpacity = Mathf.Lerp(opacity, 0f, elapsed / fadeDuration);
            hurt_overlay.color = new Color(1f, 0f, 0f, newOpacity);
            yield return new WaitForSeconds(opacity * 0.01f);
        }
        hurt_overlay.color = new Color(1f, 0f, 0f, 0f);
    }

    public float dmg(float amt)
    {
        curHealth -= amt;
        StartCoroutine(hurtOverlay(amt));
        updateInterface();
        if(curHealth <= 0) {Death();}
        return curHealth;
    }

    public float heal(float amt)
    {
        curHealth += amt;
        updateInterface();
        return curHealth;
    }

    public void restore()
    {
        curHealth = health;
        updateInterface();
    }

    public bool isAlive()
    {
        return alive;
    }
}
