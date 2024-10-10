using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mech : MonoBehaviour {
	[Serializable]
	public class Component {
		public enum Type {CORE, LEG, PRIMARY_WEAPON, SECONDARY_WEAPON};
		public String name;
		public Type type;
		public List<Hitbox> hitbox;
		public Destructable destructable;
	}

	public List<Component> components;

	private void Awake() {
		foreach(Component c in components) {
			c.destructable.OnHealthChange.AddListener(OnDestructableHealthChange);
		}
	}

	public void OnDestructableHealthChange(Destructable d) {
		foreach(Component c in components) {
			if(c.destructable == d) {
				if(d.healthCurrent == 0) {
					OnComponentDestroyed(c);
				}
				// TODO update UI
				return;
			}
		}
	}

	public void OnComponentDestroyed(Component c) {
		switch(c.type) {
			case Component.Type.CORE:
				Debug.Log("Player dead!");
				break;
			case Component.Type.LEG:
				Debug.Log("Movement speed reduced!");
				break;
			case Component.Type.PRIMARY_WEAPON:
				Debug.Log("Primary weapon destroyed!");
				break;
			case Component.Type.SECONDARY_WEAPON:
				Debug.Log("Secondary weapon destroyed!");
				break;
		}
	}
}
