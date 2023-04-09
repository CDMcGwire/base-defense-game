using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace managers {
public class PooledProvider : Provider {
#pragma warning disable 0649
	[SerializeField] private GameObject template;
	[SerializeField] private int initialInstances = 32;
	[SerializeField] private float growthFactor = 2.0f;
	[SerializeField] private int maxInstances = 1024;
#pragma warning restore 0649

	private readonly List<Reclaimer> pool = new(32);
	private int nextAvailable;
	private int loanCount;

	private void Awake() {
		if (template == null) return;
		pool.Capacity = initialInstances;
		FillPool();
	}

	/// <summary>
	/// Instantiates entries until the pool has reached its current capacity.
	/// </summary>
	private void FillPool() {
		Debug.AssertFormat(
			!@ReferenceEquals(template, null),
			"Attempted to use spawner [{0}] without a template object.",
			gameObject.name
		);
		for (var i = pool.Count; i < pool.Capacity; i++) {
			pool.Add(CreateInstance());
		}
	}

	/// <summary>
	/// Will grow the pool by the specified growth multiplier until the maximum capacity limit is
	/// reached.
	/// </summary>
	private void GrowPool() {
		if (pool.Capacity >= maxInstances)
			return;
		pool.Capacity = Math.Min((int)(pool.Capacity * growthFactor), maxInstances);
		FillPool();
	}

	/// <summary>
	/// Retrieve or replace the template GameObject for the pool. Be advised that modifying the
	/// template will invoke a rebuild of the pool and is thus expensive. It will try to create as
	/// many new objects as were in the list prior. This operation will disown existing, active
	/// entries by default, letting them be deleted by some other logic. Optionally the forceDestroy
	/// flag can be set to immediately mark all previously pooled objects for deletion. Non-active
	/// objects will always be deleted. Disowned objects will destroy implicitly when next disabled,
	/// as other that is how other systems are assumed to clean up.
	/// </summary>
	/// <param name="newTemplate">The new template GameObject to pool.</param>
	/// <param name="forceDestroy">Optional flag to force deletion of old objects.</param>
	public void SetTemplate(GameObject newTemplate, bool forceDestroy = false) {
		if (template != null) {
			foreach (var item in pool) {
				if (!item.gameObject.activeSelf || forceDestroy)
					Destroy(item);
				else
					item.Disowned = true;
			}
		}
		pool.Clear();
		template = newTemplate;
		FillPool();
	}

	/// <summary>
	/// Will attempt to get the next available pooled object. If all allocated objects have already
	/// been loaned out, then it may trigger a resize if the maximum limit has not already been
	/// reached.
	/// </summary>
	/// <returns>A reference to the stored GameObject if found, otherwise null.</returns>
	[CanBeNull]
	public override GameObject Next(bool canSteal = false) {
		if (loanCount >= pool.Capacity) {
			if (canSteal) {
				var next = pool[nextAvailable].gameObject;
				nextAvailable = (nextAvailable + 1) % pool.Count;
				return next;
			}
			GrowPool();
		}
		if (loanCount >= pool.Capacity)
			return null;

		for (var i = 0; i < pool.Count; i++) {
			var candidate = pool[nextAvailable];
			nextAvailable = (nextAvailable + 1) % pool.Count;
			if (candidate.Claimed)
				continue;
			candidate.Claimed = true;
			loanCount++;
			return candidate.gameObject;
		}
		return null;
	}

	/// <summary>
	/// Spawns an instance of the template object then prepares it for pooling.
	/// </summary>
	/// <returns>A reference to the Reclaimer component attached to the new object.</returns>
	private Reclaimer CreateInstance() {
		var instance = Instantiate(template, transform);
		instance.SetActive(false);
		var reclaimer = instance.AddComponent<Reclaimer>();
		reclaimer.Owner = this;
		return reclaimer;
	}

	/// <summary>
	/// A special MonoBehavior that the pool attaches to objects to manage their lifetime.
	/// </summary>
	public class Reclaimer : MonoBehaviour {
		internal PooledProvider Owner;
		internal bool Claimed;
		internal bool Disowned;

		private void OnDisable() {
			if (Disowned)
				Destroy(this);
			else {
				Owner.loanCount--;
				Claimed = false;
			}
		}
	}
}
}