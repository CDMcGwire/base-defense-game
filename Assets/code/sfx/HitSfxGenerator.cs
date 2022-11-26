using System;
using combat.components;
using combat.effects.core;
using UnityEngine;

namespace sfx {
/// <summary>
///   Interface for playing particle effects at a given point, intended for use
///   with damage systems.
/// </summary>
public class HitSfxGenerator : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private HitParticleCandidate[] hitParticleCandidates;
#pragma warning restore 0649

	private void OnValidate() {
		Array.Sort(hitParticleCandidates, (a, b) => a.DamageThreshold.CompareTo(b));
	}

	/// <summary>
	///   Given a hit location and a damage report, the appropriate effect will
	///   be selected and played from the contact position with the reported
	///   normal of the hit being the "up" direction of the effect.
	/// </summary>
	/// <param name="hit">Geometrical information for the triggering "hit".</param>
	/// <param name="report">Information on the damage caused.</param>
	public void PlayEffect(TargetLocation2D hit, DamageReport report) {
		var candidateIndex = -1;
		for (var i = 0; i < hitParticleCandidates.Length; i++) {
			if (hitParticleCandidates[i].DamageThreshold <= report.damage)
				candidateIndex = i;
		}
		if (candidateIndex < 0)
			return;
		hitParticleCandidates[candidateIndex]
			.HitParticle.PlayEffectFrom(hit.collider.transform, hit.point, hit.normal);
	}
}

/// <summary>
///   A struct for listing possible particle effects to play when hit. Currently
///   supports changing the effect based on the amount of damage dealt.
/// </summary>
[Serializable]
public struct HitParticleCandidate {
#pragma warning disable 0649
	[SerializeField] private HitParticle hitParticle;
	[SerializeField] private double damageThreshold;
#pragma warning restore 0649

	/// <summary>
	///   A reference to the actual HitParticle system that will run.
	/// </summary>
	public HitParticle HitParticle => hitParticle;
	/// <summary>
	///   The amount of damage that must be dealt before this system will be used.
	/// </summary>
	public double DamageThreshold => damageThreshold;
}
}