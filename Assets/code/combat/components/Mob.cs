using UnityEngine;

namespace combat.components {
public class Mob : Combatant {
#pragma warning disable 0649
	[SerializeField] private bool airborne;
#pragma warning restore 0649

	public bool Airborne => airborne;

	public void Die() => Destroy(gameObject);
}
}