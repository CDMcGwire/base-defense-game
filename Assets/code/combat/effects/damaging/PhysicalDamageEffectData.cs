using System;
using combat.effects.core;
using combat.effects.modifiers;
using data.livevalue;
using UnityEngine;

namespace combat.effects.damaging {
[CreateAssetMenu(fileName = "phys-damage-effect", menuName = "Combat/Effects/Physical Damage")]
public class PhysicalDamageEffectData : CombatEffectData<PhysicalDamageEffect> { }

[Serializable]
public class PhysicalDamageEffect : CombatEffect {
	[SerializeField] private LiveLong amount = new LiveLong(1);

	public override CombatEffect Stack(CombatEffect next, int stackCount) {
		if (next is PhysicalDamageEffect safeNext)
			return new PhysicalDamageEffect {amount = amount + safeNext.amount};
		return this;
	}

	protected override void ApplyEffect(CombatEffectResolver resolver, Combatant origin, TargetLocation2D hit) {
		if (resolver.DamageComponent != null)
			resolver.DamageComponent.Deal(amount.Current);
	}

	public override CombatEffect Modify(CombatMod mod) {
		switch (mod) {
			case AdditiveMod addMod:
				return new PhysicalDamageEffect {amount = amount + (long) addMod.Value};
			case MultiplicativeMod multMod:
				return new PhysicalDamageEffect {amount = amount * (long) multMod.Value};
			default: return this;
		}
	}
}
}