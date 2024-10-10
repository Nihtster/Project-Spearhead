using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour {
	public Collider hiboxCollider;
	public Destructable component;
	public int damageMultiplier;

	public void DealDamage(int amount) {
		component.DealDamage(amount * damageMultiplier);
	}
}
