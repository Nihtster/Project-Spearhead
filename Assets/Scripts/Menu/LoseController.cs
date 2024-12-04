using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoseController : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private TMP_Text label;
    [SerializeField] private TMP_Text btn_label;
    [SerializeField] private Button btn;

    private void onBtnClick()
    {
        
    }

    void Start()
    {
        btn.onClick.AddListener(() => onBtnClick());
    }
}
