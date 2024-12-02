using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Canvas menu;
    [SerializeField] private Button playButton;
    [SerializeField] private TextMeshProUGUI loadingText;

    private void onPlay()
    {
        menu.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(true);
        StartCoroutine(loadCustomizationScene());
    }

    private void Start()
    {
        playButton.onClick.AddListener(onPlay);
    }

    IEnumerator loadCustomizationScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("PlayerCustomization");

        while (!asyncLoad.isDone)
        {
            string load = "" + Mathf.Clamp01(asyncLoad.progress / 0.9f) * 100 + "%";
            Debug.Log(load);
            loadingText.GetComponent<TMP_Text>().text = load;
            yield return null;
        }
    }

}
