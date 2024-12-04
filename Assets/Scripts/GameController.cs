using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    
	public static GameController singleton;

	private void Awake() {
		//Set up the singleton reference
		if(singleton != null) {
			Debug.LogWarning("There are two GameControllers! Please ensure there's only one GameController in the scene at a time.");
			Destroy(this);
			return;
		} else {
			DontDestroyOnLoad(this);
			singleton = this;
		}
    }


}