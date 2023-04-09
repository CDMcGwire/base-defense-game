using managers;
using UnityEngine;

namespace sfx {
/// <summary>
///   A component responsible for managing a particle system intended for use
///   as a "hit" effect.
/// </summary>
public class HitParticle : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private Provider particleProvider;
	[Tooltip(
		"If checked, will end old particle effects prematurely to allow new ones to spawn when the limit has been reached."
	)]
	[SerializeField] private bool alwaysPlay = true;
#pragma warning restore 0649

	private void Awake() {
		Debug.AssertFormat(particleProvider != null, "HitParticle [{0}] has no particle system provider", gameObject.name);
	}

	/// <summary>
	///   Gets a pooled particle system, sets its transform parameters, then plays it.
	/// </summary>
	/// <param name="parent">The component the effect should attach to (i.e. the hit component).</param>
	/// <param name="position">Where in world space the hit occurred.</param>
	/// <param name="direction">The world space direction of the hit normal.</param>
	public void PlayEffectFrom(Transform parent, Vector2 position, Vector2 direction) {
		Debug.DrawRay(position, direction * 5.0f, Color.red, 2.0f);
		var particle = particleProvider.Next(alwaysPlay);
		if (ReferenceEquals(particle, null))
			return;
		var particleTransform = particle.transform;
		particleTransform.SetParent(null);
		// var posInLocal = parent.InverseTransformPoint(position);
		// var dirInLocal = parent.InverseTransformDirection(direction);
		var posInLocal = position;
		var dirInLocal = direction;
		particleTransform.SetLocalPositionAndRotation(posInLocal, Quaternion.LookRotation(Vector3.forward, dirInLocal));
		particle.SetActive(true);
		particle.GetComponent<ParticleSystem>().Play();
	}
}
}