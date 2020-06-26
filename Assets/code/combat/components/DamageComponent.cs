using System;
using UnityEngine;
using UnityEngine.Events;

namespace combat.components {
public class DamageComponent : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private long maxHealth = 1;
	[SerializeField] private DamageEvent onDamaged = new DamageEvent();
	[SerializeField] private DamageEvent onDestroyed = new DamageEvent();
#pragma warning restore 0649

	public DamageEvent OnDamaged => onDamaged;
	public DamageEvent OnDestroyed => onDestroyed;

	public long MaxHealth => maxHealth;
	public long CurrentHealth { get; private set; }
	public bool HasHealth => CurrentHealth > 0;
	public bool Destroyable => maxHealth > 0;
	
	private void Awake() => CurrentHealth = maxHealth;

	public void Deal(long amount) {
		DamageReport damageReport;
		damageReport.lastHealth = CurrentHealth;
		damageReport.maxHealth = maxHealth;
		damageReport.damage = amount;

		CurrentHealth -= amount;
		damageReport.currentHealth = CurrentHealth;

		onDamaged.Invoke(damageReport);
		if (damageReport.WasDestroyed)
			onDestroyed.Invoke(damageReport);
	}

	public void PrintDamageReport(DamageReport report) {
		Debug.Log($"Initial Health: {report.lastHealth}; New Health: {report.currentHealth}; Damage Dealt: {report.damage}");
	}
}

[Serializable]
public class DamageEvent : UnityEvent<DamageReport> { }

[Serializable]
public struct DamageReport {
	public long lastHealth;
	public long currentHealth;
	public long maxHealth;
	public long damage;

	public bool WasDestroyed => lastHealth > 0 && currentHealth <= 0;
}
}