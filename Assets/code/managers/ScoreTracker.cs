using System;
using combat.components;
using refevents;
using UnityEngine;

namespace managers {
public class ScoreTracker : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private RefLongEvent onScoreUpdateEvent;
#pragma warning restore 0649

	private long score = 0;

	private void Awake() {
		Debug.Assert(onScoreUpdateEvent != null, "There is no score update event reference for the score tracker.");
	}

	public void HandleMobKilled(Mob data) {
		score += 1;
		onScoreUpdateEvent.Trigger(score);
	}
}
}
