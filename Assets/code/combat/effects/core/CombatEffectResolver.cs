using System.Collections.Generic;
using combat.components;
using UnityEngine;

namespace combat.effects.core {
/// <summary>
/// Sandbox object that provides an effect with all the systems it wants to interact with
/// on a target object, if present.
/// </summary>
[RequireComponent(typeof(DefensiveModsComponent))]
public class CombatEffectResolver : MonoBehaviour {
	// TODO: Design system for supplying final target types from data.
	public DamageComponent DamageComponent { get; private set; }
	private DefensiveModsComponent DefensiveModsSource { get; set; }

	private void Awake() {
		DamageComponent = GetComponent<DamageComponent>();
		DefensiveModsSource = GetComponent<DefensiveModsComponent>();
	}

	public void ResolvePayload(
		Combatant origin,
		IReadOnlyList<CombatEffect> payload,
		IReadOnlyList<TargetLocation2D> hits
	) {
		var baseMods = DefensiveModsSource.BaseMods;
		foreach (var hit in hits) {
			ApplyPayloadAtHit(origin, payload, hit, baseMods);
		}
	}

	public void ResolvePayload(
		Combatant origin,
		IReadOnlyList<CombatEffect> payload,
		TargetLocation2D hit
	) {
		var baseMods = DefensiveModsSource.BaseMods;
		ApplyPayloadAtHit(origin, payload, hit, baseMods);
	}

	private void ApplyPayloadAtHit(
		Combatant origin,
		IReadOnlyList<CombatEffect> payload,
		TargetLocation2D hit,
		ModsByKind baseMods
	) {
		// Hit collider does not have any locational modifiers
		var locationalModSource = DefensiveModsSource.ModsAtLocation(hit.collider.GetInstanceID());
		if (locationalModSource != null) {
			var globalLocationMods = locationalModSource.Global;
			foreach (var effect in payload) {
				var locationModsOfKind = locationalModSource.OfKind(effect.Kind);
				effect.Apply(
					this,
					origin,
					hit,
					baseMods.Global,
					baseMods.OfKind(effect.Kind),
					globalLocationMods,
					locationModsOfKind
				);
			}
		}
		else {
			foreach (var effect in payload)
				effect.Apply(
					this,
					origin,
					hit,
					baseMods.Global,
					baseMods.OfKind(effect.Kind)
				);
		}
	}
}
}