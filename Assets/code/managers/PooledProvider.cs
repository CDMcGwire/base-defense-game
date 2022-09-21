using System.Collections.Generic;

using UnityEngine;

namespace managers {
public class PooledProvider : Provider {
	private readonly HashSet<int> onLoan = new();

	private readonly LinkedList<GameObject> pool = new();

	private void Awake() {
		if (lazyInstantiate) return;
		for (var i = 0; i < initialInstances; i++)
			pool.AddLast(CreateInstance());
	}

	public override GameObject Next() {
		// If the number on loan has hit the max, return nothing.
		if (onLoan.Count >= maxInstances)
			return null;
		// Can't guarantee an object wasn't destroyed for some unexpected reason
		// after being returned, so check for and discard any nulls.
		while (pool.Count > 0 && pool.First.Value == null)
			pool.RemoveFirst();

		if (pool.Count <= 0) {
			var instance = CreateInstance();
			onLoan.Add(instance.GetInstanceID());
			return instance;
		}
		else {
			var instance = pool.First.Value;
			pool.RemoveFirst();
			onLoan.Add(instance.GetInstanceID());
			return instance;
		}
	}

	internal void Return(GameObject instance) {
		if (!onLoan.Remove(instance.GetInstanceID())) return;
		pool.AddLast(instance);
	}

	private GameObject CreateInstance() {
		var instance = Instantiate(template, transform);
		instance.SetActive(false);

		var reclaimer = instance.AddComponent<Reclaimer>();
		reclaimer.owner = this;

		return instance;
	}

	public class Reclaimer : MonoBehaviour {
		internal PooledProvider owner;

		private void OnDisable() {
			owner.Return(gameObject);
		}
	}
#pragma warning disable 0649
	[SerializeField] private GameObject template;
	[SerializeField] private int initialInstances;
	[SerializeField] private int maxInstances;
	[SerializeField] private bool lazyInstantiate;
#pragma warning restore 0649
}
}