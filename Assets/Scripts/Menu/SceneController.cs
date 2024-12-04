using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneController : MonoBehaviour {
    public static SceneController singleton;
    public Image loadScreen;
    public TextMeshProUGUI text;

    private void Awake() {
        //Set up the singleton reference
        StartCoroutine(FadeToClear());
		if(singleton != null) {
			Debug.LogWarning("There are two SceneControllers! Please ensure there's only one SceneController in the scene at a time.");
			//Destroy(this);
			return;
		} else {
			DontDestroyOnLoad(this);
			singleton = this;
		}
    }

    public IEnumerator FadeToBlack() {
        loadScreen.gameObject.SetActive(true);
        text.transform.parent.gameObject.SetActive(false);
        while(loadScreen.color.a < 1) {
            loadScreen.color += new Color(0, 0, 0, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator FadeToClear() {
        loadScreen.gameObject.SetActive(true);
        text.transform.parent.gameObject.SetActive(false);
        while(loadScreen.color.a > 0) {
            loadScreen.color -= new Color(0, 0, 0, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        loadScreen.gameObject.SetActive(false);
    }

    public void LoadScene(string sceneName) {
        StartCoroutine(LoadSceneEnum(sceneName));
    }

    private IEnumerator LoadSceneEnum(string sceneName) {
        loadScreen.gameObject.SetActive(true);
        text.transform.parent.gameObject.SetActive(true);
        while(loadScreen.color.a < 1) {
            loadScreen.color += new Color(0, 0, 0, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone) {
            string load = "" + Mathf.Clamp01(asyncLoad.progress / 0.9f) * 100 + "%";
            text.text = load;
            yield return null;
        }
        StartCoroutine(FadeToClear());
    }

    public void ExitGame() {
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
