using System.Collections.Generic;
using combat.components;
using sfx;
using UnityEngine;

namespace combat.effects.core {
/// <summary>
///   Sandbox object that provides an effect with all the systems it wants to
///   interact with on a target object, if present.
/// </summary>
[RequireComponent(typeof(DefensiveModsComponent))]
public class CombatEffectResolver : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private DamageComponent damageComponent;
	[SerializeField] private DefensiveModsComponent defensiveModsSource;
	[SerializeField] private HitSfxGenerator hitSfxGenerator;
#pragma warning restore 0649

	public DamageComponent DamageComponent => damageComponent;
	public HitSfxGenerator HitSfxGenerator => hitSfxGenerator;

	private void Awake() {
		Debug.AssertFormat(
			damageComponent != null,
			"CombatEffectResolver on [{0}] does not have a damage component.",
			gameObject.name
		);
		Debug.AssertFormat(
			defensiveModsSource != null,
			"CombatEffectResolver on [{0}] does not have a damage component.",
			gameObject.name
		);
	}

	public void ResolvePayload(
		Combatant origin,
		IReadOnlyList<CombatEffect> payload,
		IReadOnlyList<TargetLocation2D> hits
	) {
		var baseMods = defensiveModsSource.BaseMods;
		foreach (var hit in hits) ApplyPayloadAtHit(origin, payload, hit, baseMods);
	}

	public void ResolvePayload(
		Combatant origin,
		IReadOnlyList<CombatEffect> payload,
		TargetLocation2D hit
	) {
		var baseMods = defensiveModsSource.BaseMods;
		ApplyPayloadAtHit(origin, payload, hit, baseMods);
	}

	private void ApplyPayloadAtHit(
		Combatant origin,
		IReadOnlyList<CombatEffect> payload,
		TargetLocation2D hit,
		ModsByKind baseMods
	) {
		// Hit collider does not have any locational modifiers
		var locationalModSource = defensiveModsSource.ModsAtLocation(hit.collider.GetInstanceID());
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