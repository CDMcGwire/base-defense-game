using UnityEngine;

namespace sfx {
public class ParticleDisabler : MonoBehaviour {
	private new ParticleSystem particleSystem;
	
	private void Awake() => particleSystem = GetComponent<ParticleSystem>();

	private void Update() {
		if (particleSystem.isStopped) gameObject.SetActive(false);
	}
}
}