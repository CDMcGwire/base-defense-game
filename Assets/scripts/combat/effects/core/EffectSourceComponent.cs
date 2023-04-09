using UnityEngine;

namespace combat.effects.core {
public class EffectSourceComponent : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private CombatEffectSource effectSource;
#pragma warning restore 0649
	public CombatEffectSource Source => effectSource;
}
}