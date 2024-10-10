using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	public int damage;
	public float velocity;

	public void FixedUpdate() {
		RaycastHit hit;
		Debug.DrawRay(transform.position, (Physics.gravity + transform.InverseTransformDirection(new Vector3(0, 0, velocity))) * Time.fixedDeltaTime, Color.red);
		if(Physics.Raycast(transform.position, (Physics.gravity + transform.InverseTransformDirection(new Vector3(0, 0, velocity))) * Time.fixedDeltaTime, out hit, (velocity + Physics.gravity.magnitude) * Time.fixedDeltaTime)) {
			if(hit.collider.gameObject.TryGetComponent(out Hitbox hitbox)) {
				hitbox.DealDamage(damage);
			}
			Destroy(gameObject);
		} else {
			transform.position += (Physics.gravity + transform.InverseTransformDirection(new Vector3(0, 0, velocity))) * Time.fixedDeltaTime;
		}
	}
}
