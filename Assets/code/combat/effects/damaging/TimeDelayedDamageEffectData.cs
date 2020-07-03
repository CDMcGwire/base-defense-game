using System;
using System.Collections;

using combat.components;
using combat.effects.core;
using combat.effects.modifiers;

using data.livevalue;

using UnityEngine;

namespace combat.effects.damaging {
[CreateAssetMenu(fileName = "delayed-damage-effect", menuName = "Combat/Effects/Delayed Damage")]
public class TimeDelayedDamageEffectData : CombatEffectData<TimeDelayedDamageEffect> { }

[Serializable]
public class TimeDelayedDamageEffect : CombatEffect {
	[SerializeField] private LiveLong amount = new LiveLong(1);
	[SerializeField] private LiveFloat delay = new LiveFloat(1f);

	public override CombatEffect Stack(CombatEffect next, int stackCount) {
		if (next is TimeDelayedDamageEffect delayedEffect)
			return new TimeDelayedDamageEffect {
				delay = new LiveFloat(
					delay.Initial,
					Math.Max(delay.Current, delayedEffect.delay.Initial)
				),
				amount = amount + delayedEffect.amount
			};
		return this;
	}

	protected override void ApplyEffect(CombatEffectResolver resolver, Combatant origin, TargetLocation2D hit) {
		resolver.StartCoroutine(DelayedEffect(resolver.DamageComponent));
	}

	public override CombatEffect Modify(CombatMod mod) {
		switch (mod) {
			case AdditiveMod addMod:
				return new TimeDelayedDamageEffect {delay = delay, amount = amount + (long) addMod.Value};
			case MultiplicativeMod multMod:
				return new TimeDelayedDamageEffect {delay = delay, amount = amount * (long) multMod.Value};
			default: return this;
		}
	}

	private IEnumerator DelayedEffect(DamageComponent damageComponent) {
		yield return new WaitForSeconds(delay.Current);
		damageComponent.Deal(amount.Current);
	}
}
}