using combat.weapon;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace ai.mob {
public class Drone : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private Weapon weapon;
#pragma warning restore 0649

	private int enemiesInSight = 0;
	private UnityAction attackEndCallback;

	private void OnEnable() {
		void AttackEndCallback() {
			if (enemiesInSight > 0) weapon.BeginAttack();
		}
		attackEndCallback = AttackEndCallback;
		weapon.OnAttackEnd.AddListener(attackEndCallback);
	}

	private void OnDisable() => weapon.OnAttackEnd.RemoveListener(attackEndCallback);

	[UsedImplicitly]
	public void OnEnemySighted() {
		if (enemiesInSight < 1) weapon.BeginAttack();
		enemiesInSight++;
	}

	[UsedImplicitly]
	public void OnEnemyLost() {
		enemiesInSight--;
		if (enemiesInSight < 1) weapon.ReleaseAttack();
	}
}
}