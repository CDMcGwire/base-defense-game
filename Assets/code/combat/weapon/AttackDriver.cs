using UnityEngine;

namespace combat.weapon {
public abstract class AttackDriver : MonoBehaviour {
	public Weapon Weapon { get; set; }
	public abstract void Begin();
	public abstract void Release();
}
}