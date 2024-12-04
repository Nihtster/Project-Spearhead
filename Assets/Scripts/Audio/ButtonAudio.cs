using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public string enter;
    public string exit;
    public string click;
    public string deny;

    private Button b;

    private void Awake() {
        b = GetComponent<Button>();
        b.onClick.AddListener(OnButtonPressed);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if(enter == "") { return; }
        AudioController.PlaySound(enter, Camera.main.transform);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if(exit == "") { return; }
        AudioController.PlaySound(exit, Camera.main.transform);
    }

    private void OnButtonPressed() {
        if(click == "") { return; }
        if(click == "" && deny == "") { return; }
        if(b.interactable && click != "") {
            AudioController.PlaySound(click, Camera.main.transform);
        } else if(!b.interactable && deny != "") {
            AudioController.PlaySound(deny, Camera.main.transform);
        }
    }


}
