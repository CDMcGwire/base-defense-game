using System;
using System.Collections.Generic;

using combat.components;

using UnityEngine;

namespace combat.effects.core {
[Serializable]
public abstract class CombatEffect : PayloadObject<CombatEffect> {
	protected abstract void ApplyEffect(CombatEffectResolver resolver, Combatant origin, TargetLocation2D hit);

	public void Apply(
		CombatEffectResolver resolver,
		Combatant origin,
		TargetLocation2D hit,
		params IEnumerable<CombatMod>[] additionalMods
	) {
		AddMods(additionalMods).ApplyEffect(resolver, origin, hit);
	}

	/// <summary>
	///   If the mod is compatible, this method will create a new of the Effect with
	///   new effect values.
	/// </summary>
	/// <param name="mod">
	///   A <see cref="CombatMod" /> that should be applied to the effect.
	/// </param>
	/// <returns>
	///   A CombatEffect that has been modified according to its mod logic.
	/// </returns>
	public abstract CombatEffect Modify(CombatMod mod);

	/// <summary>
	///   Adds all mods in the given chain to the to the effect. This method modifies
	///   the effect in place. If the modification should be temporary, use
	///   AddTempMods.
	/// </summary>
	/// <param name="modChains">A collection of <see cref="CombatMod" /> objects.</param>
	public CombatEffect AddMods(params IEnumerable<CombatMod>[] modChains) {
		var finalEffect = this;
		foreach (var chain in modChains)
		foreach (var mod in chain)
			finalEffect = finalEffect.Modify(mod);
		return finalEffect;
	}
}

public abstract class CombatEffectData<T> : PayloadDataSource<CombatEffect> where T : CombatEffect {
#pragma warning disable 0649
	[SerializeField] private T effect;
#pragma warning restore 0649
	public override CombatEffect Value => effect;
}
}