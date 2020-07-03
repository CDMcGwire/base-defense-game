using System;
using System.Collections.Generic;

using combat.components;
using combat.effects.core;
using combat.targeting;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace combat.weapon {
public class Weapon : MonoBehaviour {
	private readonly HashSet<int> activeTargets = new HashSet<int>();
	private readonly HashSet<int> ignoredTargets = new HashSet<int>();

	private readonly HashSet<int> targetFilter = new HashSet<int>();

	public Combatant Owner { get; set; }
	public WeaponEvent OnAttackEnd => onAttackEnd;

	private void Awake() {
		if (driver) driver.Weapon = this;
		TargetFilter.Build(filteredTargetTypes, targetFilter);
	}

	public void BeginAttack() {
		driver.Begin();
	}

	public void ReleaseAttack() {
		driver.Release();
	}

	public void HandleInput(InputAction.CallbackContext context) {
		if (context.started)
			driver.Begin();
		else if (context.performed)
			driver.Release();
	}

	public void HandleTarget(TargetLocation2D target, IReadOnlyList<CombatEffect> payload) {
		// Target was already reported once since the last refresh and was ignored.
		if (ignoredTargets.Contains(target.id)) return;
		// Check if the target has a resolver. If not, ignore it.
		var resolver = GetTargetResolver(target);
		if (ReferenceEquals(resolver, null)) {
			ignoredTargets.Add(target.id);
			return;
		}
		// Check if the resolver was already filtered or was already targeted.
		var resolverId = resolver.GetInstanceID();
		if (ignoredTargets.Contains(resolverId)
		    || activeTargets.Contains(resolverId))
			return;
		// Check if it's a filtered target type.
		var typeHash = resolver.tag.GetHashCode();
		if (targetFilter.Contains(typeHash)) {
			ignoredTargets.Add(resolverId);
			return;
		}
		// Add to list of active targets for this refresh cycle and resolve the payload.
		activeTargets.Add(resolver.GetInstanceID());
		resolver.ResolvePayload(Owner, payload, target);
	}

	public void ClearTargetingMemory() {
		activeTargets.Clear();
		ignoredTargets.Clear();
	}

	private static CombatEffectResolver GetTargetResolver(TargetLocation2D hit) {
		return ReferenceEquals(hit.rigidbody, null)
			? hit.collider.GetComponent<CombatEffectResolver>()
			: hit.rigidbody.GetComponent<CombatEffectResolver>();
	}
#pragma warning disable 0649
	[SerializeField] private List<string> filteredTargetTypes = new List<string>();
	[SerializeField] private AttackDriver driver;
	[SerializeField] private WeaponEvent onAttackEnd = new WeaponEvent();
#pragma warning restore 0649
}

public readonly struct AttackInfo {
	public readonly Combatant attacker;
	public readonly Weapon weapon;
	public readonly IReadOnlyCollection<string> targetFilters;
	public readonly IReadOnlyList<CombatEffect> payload;

	public AttackInfo(Combatant attacker, Weapon weapon, IReadOnlyCollection<string> targetFilters, IReadOnlyList<CombatEffect> payload) {
		this.attacker = attacker;
		this.weapon = weapon;
		this.targetFilters = targetFilters;
		this.payload = payload;
	}
}

[Serializable]
public class WeaponEvent : UnityEvent { }
}