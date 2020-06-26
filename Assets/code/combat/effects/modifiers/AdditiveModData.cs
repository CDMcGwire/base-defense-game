using System;
using combat.effects.core;
using UnityEngine;

namespace combat.effects.modifiers {
[CreateAssetMenu(fileName = "additive-mod", menuName = "Combat/Modifiers/Additive")]
public class AdditiveModData : CombatModData<AdditiveMod> { }

[Serializable]
public class AdditiveMod : CombatMod {
	[SerializeField] private double value;
	public double Value => value;

	public override CombatMod Stack(CombatMod next, int stackCount) {
		if (next is AdditiveMod addMod) value += addMod.value;
		return this;
	}
}
}