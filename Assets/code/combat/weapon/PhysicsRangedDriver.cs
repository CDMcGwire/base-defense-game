using sfx;

using UnityEngine;

namespace combat.weapon {
public class PhysicsRangedDriver : RangedAttackDriver {
	protected override void OnFire() {
		spawner.SpawnWorld(projectile);
	}
#pragma warning disable 0649
	[SerializeField] private GameObject projectile;
	[SerializeField] private Spawner spawner;
#pragma warning restore 0649
}
}