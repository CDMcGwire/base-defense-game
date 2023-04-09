using UnityEngine;

namespace combat.effects.core {
public class ModSourceComponent : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private CombatModSource modSource;
#pragma warning restore 0649
	public CombatModSource Source => modSource;
}
}