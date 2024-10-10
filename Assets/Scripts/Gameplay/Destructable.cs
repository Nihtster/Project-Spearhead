using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using OpenCover.Framework.Model;
using UnityEngine;
using UnityEngine.Events;
public class Destructable : MonoBehaviour {
	[System.Serializable]
	public class HealthChangeEvent : UnityEvent<Destructable> {}
	public int healthCurrent;
	public int healthMax;
	public HealthChangeEvent OnHealthChange;
	
	///<summary>Deals a given amount of damage (or heals, if amount is negative)</summary>
	///<param name="amount">Deals (or heals) a given amount of damage</param>
	public void DealDamage(int amount) {
		healthCurrent -= amount;
		OnHealthChange.Invoke(this);
		if(healthCurrent <= 0) {
			healthCurrent = 0;
		} else if(healthCurrent > healthMax) {
			healthCurrent = healthMax;
		}
	}
}
