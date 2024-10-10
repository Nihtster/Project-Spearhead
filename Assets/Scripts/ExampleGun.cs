using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleGun : MonoBehaviour {
	public float velocity = 100;
	public int damage = 10;
	private void Update() {
		if(Input.GetMouseButtonDown(0)) {
			GameObject go = new("bullet");
			go.transform.parent = null;
			go.transform.position = transform.position;
			go.transform.eulerAngles = transform.eulerAngles;
			Projectile p = go.AddComponent<Projectile>();
			p.velocity = velocity;
			p.damage = damage;
		}
	}
}
