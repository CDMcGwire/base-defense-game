using System;

using combat.effects.core;

using UnityEngine;

namespace combat.effects.modifiers {
[CreateAssetMenu(fileName = "multiplicative-mod", menuName = "Combat/Modifiers/Multiplicative")]
public class MultiplicativeModData : CombatModData<MultiplicativeMod> { }

[Serializable]
public class MultiplicativeMod : CombatMod {
	[Header("Multiplier")] [SerializeField]
	private double value;

	public double Value => value;

	public override CombatMod Stack(CombatMod next, int stackCount) {
		if (next is MultiplicativeMod multMod) value *= multMod.value;
		return this;
	}
}
}