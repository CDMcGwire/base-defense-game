using combat.components;
using UnityEngine;

namespace managers {
public class LevelCleanup : MonoBehaviour {
	public void Cleanup() {
		foreach (var mob in FindObjectsOfType<Mob>()) 
			mob.Die();
	}
}
}
