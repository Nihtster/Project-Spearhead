using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>Handles collision sounds for scrap from a destroyed tank</summary>
public class CollisionSounds : MonoBehaviour {
	public float pitch = 1f;
	private void OnCollisionEnter(Collision other) {
		AudioController.PlaySound("scrap_impact", transform);
	}
}
