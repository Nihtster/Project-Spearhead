using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>Start environment sounds or music depending on which scene is loaded</summary>
public class SceneSounds : MonoBehaviour {
	[Tooltip("-1 = Don't play sounds\n0 = Main menu\n1 = Firing Range\n2 = Tower of Turrets")]
	public int scene = -1;
	private void Start() {
		if (scene == 0 || scene == 1) {
			AudioController.PlaySound("forest");
		}

	}
}
