using combat.effects.core;
using UnityEngine;

namespace combat.targeting {
public class HitScanTargeter : Targeter {
#pragma warning disable 0649
	[SerializeField] private Transform scanOrigin;
	[SerializeField] private LayerMask scanLayer;
	[SerializeField] private float scanRange = 1000.0f;
#pragma warning restore 0649

	private void Awake() {
		if (scanOrigin == null) scanOrigin = transform;
	}

	public override void Activate() {
		var hit = Physics2D.Raycast(
			scanOrigin.position,
			scanOrigin.right,
			scanRange,
			scanLayer
		);
		if (!hit) return;
		Weapon.HandleTarget(
			new TargetLocation2D(hit),
			EffectPool.Payload
		);
	}
}
}