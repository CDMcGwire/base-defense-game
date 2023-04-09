using combat.targeting;

using UnityEngine;

namespace combat.weapon {
public class HitScanRangedDriver : RangedAttackDriver {
#pragma warning disable 0649
	[SerializeField] private HitScanTargeter targeter;
#pragma warning restore 0649

	protected override void OnFire() {
		targeter.Activate();
	}
}
}