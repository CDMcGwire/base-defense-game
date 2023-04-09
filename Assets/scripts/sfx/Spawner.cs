using managers;
using UnityEngine;

namespace sfx {
public class Spawner : MonoBehaviour {
	public GameObject SpawnWorld(GameObject spawnable) {
		if (spawnable == null) return null;
		var tfm = transform;
		var go = Instantiate(spawnable, tfm.position, tfm.rotation);
		go.SetActive(true);
		return go;
	}

	public GameObject SpawnWorld(Provider provider) {
		return SpawnFromProvider(provider, true);
	}

	public GameObject SpawnLocal(GameObject spawnable) {
		if (spawnable == null) return null;
		var go = Instantiate(spawnable, transform);
		go.SetActive(true);
		return go;
	}

	public GameObject SpawnLocal(Provider provider) {
		return SpawnFromProvider(provider, false);
	}

	private GameObject SpawnFromProvider(Provider provider, bool worldSpace) {
		if (ReferenceEquals(provider, null))
			return null;
		var spawnable = provider.Next();
		if (ReferenceEquals(spawnable, null))
			return null;

		var tfmThis = transform;
		var tfmOther = spawnable.transform;
		if (worldSpace)
			tfmOther.SetParent(null);
		tfmOther.position = tfmThis.position;
		tfmOther.rotation = tfmThis.rotation;
		spawnable.SetActive(true);
		return spawnable;
	}
}
}