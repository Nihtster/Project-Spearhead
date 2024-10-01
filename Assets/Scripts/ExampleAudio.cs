using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleAudio : MonoBehaviour {
	void Update() {
		if(Input.GetKeyDown(KeyCode.Space)) {
			AudioController.PlaySound("test_sound");
		}
	}
}
