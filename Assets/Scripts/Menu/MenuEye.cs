using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LensFlare))]
public class MenuEye : MonoBehaviour {
    [Range(0, 10)]
    public float moveRadius;
    public float moveMin;
    public float moveMax;
    public float moveSpeed;
    public float blinkMin;
    public float blinkMax;
    public float blinkSpeed;
    public float blinkDuration;
    public float pulseRate;
    public float pulsePeak;
    public float pulseTrough;

    public float bootDuration;

    private Vector3 origin;
    private Vector3 target;
    private LensFlare lf;

    private void Start() {
        origin = transform.position;
        target = origin;
        lf = GetComponent<LensFlare>();
        lf.brightness = 0;
        StartCoroutine(BootUp());
    }

    private void Update() {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    private IEnumerator BootUp() {
        float t = 0;
        while(t < bootDuration) {
            lf.brightness += pulsePeak / bootDuration * Time.deltaTime;
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(EyeWobble());
        StartCoroutine(Blink());
        StartCoroutine(Pulse());
    }

    private IEnumerator Blink() {
        while(true) {
            yield return new WaitForSeconds(Random.Range(blinkMin, blinkMax));
            while(lf.color.r > 0) {
                lf.color = new Color(lf.color.r - (blinkSpeed * Time.deltaTime), 0, 0, 1);
                yield return new WaitForEndOfFrame();
            }
            lf.color = Color.black;
            yield return new WaitForSeconds(blinkDuration);
            while(lf.color.r < 1) {
                lf.color = new Color(lf.color.r + (blinkSpeed * Time.deltaTime), 0, 0, 1);
                yield return new WaitForEndOfFrame();
            }
            lf.color = Color.red;
        }
    }

    private IEnumerator EyeWobble() {
        while(true) {
            yield return new WaitForSeconds(Random.Range(moveMin, moveMax));
            target = origin + (Random.insideUnitSphere * moveRadius);
        }
    }

    private IEnumerator Pulse() {
        float start = Time.time;
        while(true) {
            yield return new WaitForEndOfFrame();
            float t = (Mathf.Sin((Time.time - start) * pulseRate) + 1f)/2f;
            lf.brightness = pulseTrough + (pulsePeak - pulseTrough) * t;
        }
    }
}
