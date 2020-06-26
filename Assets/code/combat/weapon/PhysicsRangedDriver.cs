using sfx;
using UnityEngine;

namespace combat.weapon {
public class PhysicsRangedDriver : RangedAttackDriver {
#pragma warning disable 0649
	[SerializeField] private GameObject projectile;
	[SerializeField] private Spawner spawner;
#pragma warning restore 0649

	protected override void OnFire() => spawner.SpawnWorld(projectile);
}
}