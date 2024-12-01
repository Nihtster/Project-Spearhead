using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CustomizationController : MonoBehaviour
{
    [SerializeField] private Canvas mechSelection;
    [SerializeField] private Button mechSubmission;
    [SerializeField] private GameObject mech;
    [SerializeField] private Canvas primarySelection;
    [SerializeField] private Button primarySubmission;
    [SerializeField] private GameObject primary;
    [SerializeField] private Canvas secondarySelection;
    [SerializeField] private Button secondSubmission;
    [SerializeField] private GameObject secondary;
    [SerializeField] private TextMeshProUGUI loadingText;

    private void onMechSubmission() {
        mechSelection.gameObject.SetActive(false);
        mech.SetActive(false);
        primarySelection.gameObject.SetActive(true);
    }

    private void onPrimarySubmission() {
        primarySelection.gameObject.SetActive(false);
        primary.SetActive(false);
        secondarySelection.gameObject.SetActive(true);
    }
    
    private void onSecondarySubmission()
    {
        secondarySelection.gameObject.SetActive(false);
        secondary.SetActive(false);
        loadingText.gameObject.SetActive(true);
        StartCoroutine(loadGameScene());
    }

    private void Start()
    {
        mechSubmission.onClick.AddListener(onMechSubmission);
        primarySubmission.onClick.AddListener(onPrimarySubmission);
        secondSubmission.onClick.AddListener(onSecondarySubmission);
    }

    IEnumerator loadGameScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("World");

        while (!asyncLoad.isDone)
        {
            string load = "" + Mathf.Clamp01(asyncLoad.progress / 0.9f) * 100 + "%";
            Debug.Log(load);
            loadingText.GetComponent<TMP_Text>().text = load;
            yield return null;
        }
    }
}
