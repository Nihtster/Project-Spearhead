using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private TMP_Text health_interface;
    [SerializeField] private Image hurt_overlay;
    [SerializeField] private float health = 100f;
    private int interval = 2; // health / interval = # of health bars
    private float curHealth;

    private void death() 
    {

    }

    void Start() { curHealth = health; updateInterface(); } //StartCoroutine(TestHealthInterface()); }

    private IEnumerator TestHealthInterface(){
        yield return new WaitForSeconds(3);
        dmg(10);
        yield return new WaitForSeconds(6);
        dmg(health / 4);
        yield return new WaitForSeconds(6);
        for (int i = 0; i < 20; i++)
        {
            curHealth -= 5;
            updateInterface();
            yield return new WaitForSeconds(2);

        }
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
        if(curHealth <= 0) {death();}
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
}
