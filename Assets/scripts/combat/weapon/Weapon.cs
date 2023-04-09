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
#pragma warning disable 0649
	[SerializeField] private List<string> filteredTargetTypes = new();
	[SerializeField] private AttackDriver driver;
	[SerializeField] private WeaponEvent onAttackEnd = new();
	[SerializeField] private Transform muzzlePoint;
#pragma warning restore 0649
	
	private readonly HashSet<int> targetFilter = new();

	public Combatant Owner { get; set; }
	public Transform MuzzlePoint => muzzlePoint;
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
		// Check if the target has a resolver. If not, ignore it.
		var resolver = GetTargetResolver(target);
		if (ReferenceEquals(resolver, null))
			return;
		resolver.ResolvePayload(Owner, payload, target);
	}

	private static CombatEffectResolver GetTargetResolver(TargetLocation2D hit) {
		return ReferenceEquals(hit.rigidbody, null)
			? hit.collider.GetComponent<CombatEffectResolver>()
			: hit.rigidbody.GetComponent<CombatEffectResolver>();
	}
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