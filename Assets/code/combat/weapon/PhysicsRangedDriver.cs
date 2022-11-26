using managers;
using sfx;
using UnityEngine;

namespace combat.weapon {
public class PhysicsRangedDriver : RangedAttackDriver {
#pragma warning disable 0649
	[SerializeField] private Provider projectileProvider;
	[SerializeField] private Spawner spawner;
#pragma warning restore 0649
	
	protected override void OnFire() {
		_ = spawner.SpawnWorld(projectileProvider);
	}
}
}