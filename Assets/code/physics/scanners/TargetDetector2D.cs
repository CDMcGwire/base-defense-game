using System;
using System.Collections.Generic;
using combat.targeting;
using UnityEngine;
using UnityEngine.Events;

namespace physics.scanners {
public class TargetDetector2D : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private List<string> filteredTargetTypes = new List<string>();
	[SerializeField] private TargetDetectedEvent onDetected;
	[SerializeField] private TargetDetectedEvent onLost;
#pragma warning restore 0649
	
	private readonly HashSet<int> targetFilter = new HashSet<int>();

	private void Awake() => TargetFilter.Build(filteredTargetTypes, targetFilter);

	private void OnTriggerEnter2D(Collider2D other) {
		var typeId = other.tag.GetHashCode();
		if (!targetFilter.Contains(typeId)) onDetected.Invoke();
	}

	private void OnTriggerExit2D(Collider2D other) {
		var typeId = other.tag.GetHashCode();
		if (!targetFilter.Contains(typeId)) onLost.Invoke();
	}
}

[Serializable]
public class TargetDetectedEvent : UnityEvent { }
}