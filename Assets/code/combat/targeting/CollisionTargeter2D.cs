using System.Collections.Generic;

using combat.effects.core;

using UnityEngine;

namespace combat.targeting {
public class CollisionTargeter2D : Targeter {
	private readonly List<ContactPoint2D> contactPointsHolder = new List<ContactPoint2D>();

	private void Awake() {
		if (GetComponent<Collider2D>() == null)
			Debug.LogWarning($"ImpactZone Component on {name} has no Collider2D");
	}

	private void OnCollisionEnter2D(Collision2D other) {
		other.GetContacts(contactPointsHolder);

		for (var i = 0; i < other.contactCount; i++) {
			var contact = other.GetContact(i);
			var targetLocation = new TargetLocation2D(
				contact.collider,
				contact.point,
				contact.normal,
				contact.otherCollider.bounds.center,
				contact.rigidbody
			);

			Weapon.HandleTarget(targetLocation, EffectPool.Payload);
		}
	}

	public override void Activate() {
		enabled = true;
	}
}
}