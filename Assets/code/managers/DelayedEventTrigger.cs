using System;
using refevents;
using UnityEngine;

namespace managers {
public class DelayedEventTrigger : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private RefEvent refEvent;
	[SerializeField] private float delay;
	[SerializeField] private bool scaled;
#pragma warning restore 0649

	private float remainingTime = 0;

	private void Update() {
		if (remainingTime > 0) {
			remainingTime -= scaled
				? Time.deltaTime
				: Time.unscaledDeltaTime;

			if (!(remainingTime <= 0)) return;
			refEvent.Trigger();
			enabled = false;
		}
		else {
			enabled = false;
		}
	}

	public void Trigger() {
		if (enabled) return;
		enabled = true;
		remainingTime = delay;
	}
}
}
