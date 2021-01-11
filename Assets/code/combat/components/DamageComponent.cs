using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace combat.components {
/// <summary>
///   <para>
///     A component that can be attached to a <see cref="GameObject" /> to make it
///     damageable by other systems. Primarily the CombatEffectResolver.
///   </para>
///   <para>
///     The component does not define the downstream response to damage, but it
///     does provide event hooks and data other components can attach to.
///   </para>
/// </summary>
public class DamageComponent : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private long maxHealth = 1;
	[SerializeField] private long initialDamage = 0;
	[SerializeField] private DamageEvent onDamaged = new DamageEvent();
	[SerializeField] private DamageEvent onDestroyed = new DamageEvent();
#pragma warning restore 0649

	private long damage;

	/// <summary>
	///   The event fired every time damage is dealt. Regardless of value dealt or
	///   start and end health.
	/// </summary>
	[UsedImplicitly] public DamageEvent OnDamaged => onDamaged;

	/// <summary>
	///   An event fired only when the damage dealt caused the health value to go below
	///   zero.
	/// </summary>
	[UsedImplicitly] public DamageEvent OnDestroyed => onDestroyed;

	public long MaxHealth {
		get => maxHealth;
		set {
			var diff = maxHealth - CurrentHealth;
			maxHealth = value;
			Damage = maxHealth > 0
				? diff >= maxHealth
					? 1
					: maxHealth - diff
				: maxHealth;
		}
	}

	public long Damage {
		get => damage;
		set => damage = value >= 0 ? value : 0;
	}

	public long CurrentHealth => maxHealth - Damage;
	public bool Destroyable => maxHealth > 0;
	public bool Destroyed => Destroyable && CurrentHealth <= 0;

	private void Awake() => Damage = initialDamage;

	/// <summary>Method called by external systems to apply damage.</summary>
	/// <param name="amount">The amount of damage to deal.</param>
	public void Deal(long amount) {
		DamageReport damageReport;
		damageReport.lastHealth = CurrentHealth;
		damageReport.maxHealth = maxHealth;
		damageReport.damage = amount;

		Damage += amount;
		damageReport.currentHealth = CurrentHealth;

		onDamaged.Invoke(damageReport);
		if (damageReport.WasDestroyed)
			onDestroyed.Invoke(damageReport);
	}

	/// <summary>Debug method that can be called to log a damage report.</summary>
	/// <param name="report">A <see cref="DamageReport" /> struct.</param>
	[UsedImplicitly]
	public void PrintDamageReport(DamageReport report) {
		Debug.Log(
			$"Initial Health: {report.lastHealth}; New Health: {report.currentHealth}; Damage Dealt: {report.damage}"
		);
	}
}

/// <summary>
///   A <see cref="UnityEvent{T0}" /> variant accepting a
///   <see cref="DamageReport" /> parameter.
/// </summary>
[Serializable]
public class DamageEvent : UnityEvent<DamageReport> { }

/// <summary>A struct representing the data relevant to damage being dealt.</summary>
[Serializable]
public struct DamageReport {
	/// <summary>The health value the component was at before damage was dealt.</summary>
	public long lastHealth;
	/// <summary>The health value the component is at now after damage was dealt.</summary>
	public long currentHealth;
	/// <summary>The maximum health value the component can hold.</summary>
	public long maxHealth;
	/// <summary>The amount of damage dealt to the component.</summary>
	public long damage;

	/// <summary>True if this was the event that destroyed the component.</summary>
	public bool WasDestroyed => lastHealth > 0 && currentHealth <= 0;
}
}