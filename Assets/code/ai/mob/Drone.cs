using combat.weapon;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.Events;

namespace ai.mob {
/// <summary>
///   <para>
///     A simple combat "AI" driver for game actors that tracks when a valid target
///     has entered range and begins attacking as long as more than one enemy is
///     still detected.
///   </para>
///   <para>The method of detection is</para>
/// </summary>
public class Drone : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private Weapon weapon;
#pragma warning restore 0649

	private UnityAction attackEndCallback;
	private int enemiesInSight;

	private void OnEnable() {
		attackEndCallback = () => {
			if (enemiesInSight > 0) weapon.BeginAttack();
		};
		weapon.OnAttackEnd.AddListener(attackEndCallback);
	}

	private void OnDisable() {
		weapon.OnAttackEnd.RemoveListener(attackEndCallback);
	}

	/// <summary>
	/// Method to be called by detector component to register a new enemy.
	/// </summary>
	[UsedImplicitly]
	public void OnEnemySighted() {
		if (enemiesInSight < 1) weapon.BeginAttack();
		enemiesInSight++;
	}

	/// <summary>
	/// Method to be called by detector when a tracked enemy is lost.
	/// </summary>
	[UsedImplicitly]
	public void OnEnemyLost() {
		enemiesInSight--;
		if (enemiesInSight < 1) weapon.ReleaseAttack();
	}
}
}