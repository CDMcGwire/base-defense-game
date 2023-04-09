using JetBrains.Annotations;
using UnityEngine;

namespace combat.components {
public class Mob : Combatant {
#pragma warning disable 0649
	[SerializeField] private bool airborne;
	[SerializeField] private string deathAnimTrigger = "die";
#pragma warning restore 0649

	private Animator animator;
	private new Rigidbody2D rigidbody;

	public bool Airborne => airborne;

	private void Awake() {
		animator = GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody2D>();
	}

	public void Die() {
		if (animator != null) {
			animator.SetTrigger(deathAnimTrigger);
			if (rigidbody != null)
				rigidbody.simulated = false;
		}
		else Remove();
	}

	[UsedImplicitly]
	public void Remove() {
		Destroy(gameObject);
	}
}
}
