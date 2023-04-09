using System.Collections.Generic;
using combat.effects.core;
using UnityEngine;
using UnityEngine.Events;

namespace combat.targeting {
public class CollisionTargeter2D : Targeter {
#pragma warning disable 0649
	[SerializeField] private UnityEvent afterCollision;
	[SerializeField] private MultiContactHandling multiContactHandling = MultiContactHandling.FirstOnly;
	[SerializeField] private bool allowPierce = false;
#pragma warning restore 0649

	private readonly List<ContactPoint2D> contactPointsHolder = new();
	private bool hasHit = false;

	private void Awake() {
		if (GetComponent<Collider2D>() == null)
			Debug.LogWarning($"ImpactZone Component on {name} has no Collider2D");
	}

	private void OnCollisionEnter2D(Collision2D other) {
		if (hasHit && !allowPierce)
			return;
		other.GetContacts(contactPointsHolder);

		ContactPoint2D contact;
		switch (multiContactHandling) {
			case MultiContactHandling.FirstOnly:
				contact = other.GetContact(0);
				Weapon.HandleTarget(
					new TargetLocation2D(
						contact.collider,
						contact.point,
						contact.normal,
						contact.otherCollider.bounds.center,
						contact.rigidbody
					),
					EffectPool.Payload
				);
				break;
			case MultiContactHandling.LastOnly:
				contact = other.GetContact(other.contactCount - 1);
				Weapon.HandleTarget(
					new TargetLocation2D(
						contact.collider,
						contact.point,
						contact.normal,
						contact.otherCollider.bounds.center,
						contact.rigidbody
					),
					EffectPool.Payload
				);
				break;
			case MultiContactHandling.All:
				for (var i = 0; i < other.contactCount; i++) {
					contact = other.GetContact(i);
					var targetLocation = new TargetLocation2D(
						contact.collider,
						contact.point,
						contact.normal,
						contact.otherCollider.bounds.center,
						contact.rigidbody
					);

					Weapon.HandleTarget(targetLocation, EffectPool.Payload);
				}
				break;
		}
		hasHit = true;
		afterCollision.Invoke();
	}

	public override void Activate() {
		enabled = true;
	}
}

public enum MultiContactHandling {
	FirstOnly,
	LastOnly,
	All
}
}