using managers;
using UnityEngine;

namespace sfx {
public class Spawner : MonoBehaviour {
	public void SpawnWorld(GameObject spawnable) {
		if (spawnable == null) return;
		var tfm = transform;
		Instantiate(spawnable, tfm.position, tfm.rotation)
			.SetActive(true);
	}

	public void SpawnWorld(Provider provider) {
		SpawnFromProvider(provider, true);
	}

	public void SpawnLocal(GameObject spawnable) {
		if (spawnable == null) return;
		Instantiate(spawnable, transform)
			.SetActive(true);
	}

	public void SpawnLocal(Provider provider) {
		SpawnFromProvider(provider, false);
	}

	private void SpawnFromProvider(Provider provider, bool worldSpace) {
		if (provider == null) return;
		var spawnable = provider.Next();
		if (spawnable == null) return;

		var tfmThis = transform;
		var tfmOther = spawnable.transform;
		tfmOther.SetParent(worldSpace ? null : tfmOther);
		tfmOther.position = tfmThis.position;
		tfmOther.rotation = tfmThis.rotation;
		spawnable.SetActive(true);
	}
}
}