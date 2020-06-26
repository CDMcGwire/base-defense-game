using combat.effects.core;
using combat.weapon;
using UnityEngine;

namespace combat.targeting {
public abstract class Targeter : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private Weapon weapon;
	[SerializeField] private CombatEffectPool effectPool;
#pragma warning restore 0649

	protected Weapon Weapon => weapon;
	protected CombatEffectPool EffectPool => effectPool;

	public abstract void Activate();
}
}