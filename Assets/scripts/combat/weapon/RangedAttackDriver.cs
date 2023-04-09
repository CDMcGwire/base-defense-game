using System;
using System.Collections.Generic;
using UnityEngine;

namespace combat.weapon {
public abstract class RangedAttackDriver : AttackDriver {
#pragma warning disable 0649
	[Header("Firing Characteristics")]
	[SerializeField] private FireMode fireMode = FireMode.Semi;
	[SerializeField] private float minInterval = 0.05f;
	[SerializeField] private int roundsPerShot = 1;
	[SerializeField] private int roundsPerBurst = 3;

	[Space(8)] [Header("Effects")]
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioClip shotSound;
	[SerializeField] private List<ParticleSystem> particleSystems;
#pragma warning restore 0649

	private int remainingBurstRounds;

	private float shotTimer;
	private bool triggerHeld;

	private void Update() {
		if (shotTimer > 0) {
			shotTimer -= Time.deltaTime;
			if (shotTimer > 0) return;
		}

		switch (fireMode) {
			case FireMode.Auto: {
				if (triggerHeld) Fire();
				break;
			}
			case FireMode.Burst: {
				if (remainingBurstRounds > 0) Fire();
				break;
			}
			case FireMode.Semi:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public override void Begin() {
		triggerHeld = true;
		if (fireMode == FireMode.Burst && remainingBurstRounds <= 0)
			remainingBurstRounds = roundsPerBurst;
		Fire();
	}

	public override void Release() {
		triggerHeld = false;
	}

	private void Fire() {
		if (shotTimer > 0) return;
		for (var i = 0; i < roundsPerShot; i++) OnFire();
		foreach (var system in particleSystems) 
			system.Emit(1);
		if (audioSource != null)
			audioSource.PlayOneShot(shotSound);
		shotTimer = minInterval;
		if (remainingBurstRounds > 0) remainingBurstRounds--;
	}

	protected abstract void OnFire();
	
	[Serializable]
	public enum FireMode {
		Auto,
		Semi,
		Burst,
	}
}
}