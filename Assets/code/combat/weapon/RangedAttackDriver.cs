using System;

using UnityEngine;

namespace combat.weapon {
public abstract class RangedAttackDriver : AttackDriver {
	[Serializable]
	public enum FireMode {
		AUTO,
		SEMI,
		BURST
	}

	private int remainingBurstRounds;

	private float shotTimer;
	private bool triggerHeld;

	private void Update() {
		if (shotTimer > 0) {
			shotTimer -= Time.deltaTime;
			if (shotTimer > 0) return;
		}

		switch (fireMode) {
			case FireMode.AUTO: {
				if (triggerHeld) Fire();
				break;
			}
			case FireMode.BURST: {
				if (remainingBurstRounds > 0) Fire();
				break;
			}
			case FireMode.SEMI:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public override void Begin() {
		triggerHeld = true;
		if (fireMode == FireMode.BURST && remainingBurstRounds <= 0)
			remainingBurstRounds = roundsPerBurst;
		Fire();
	}

	public override void Release() {
		triggerHeld = false;
	}

	private void Fire() {
		if (shotTimer > 0) return;
		for (var i = 0; i < roundsPerShot; i++) OnFire();
		particleSystem.Emit(1);
		audioSource.PlayOneShot(shotSound);
		shotTimer = minInterval;
		if (remainingBurstRounds > 0) remainingBurstRounds--;
	}

	protected abstract void OnFire();
#pragma warning disable 0649
	[Header("Firing Characteristics")]
	[SerializeField] private FireMode fireMode = FireMode.SEMI;
	[SerializeField] private float minInterval = 0.05f;
	[SerializeField] private int roundsPerShot = 1;
	[SerializeField] private int roundsPerBurst = 3;

	[Space(8)] [Header("Effects")]
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioClip shotSound;
	[SerializeField] private new ParticleSystem particleSystem;
#pragma warning restore 0649
}
}