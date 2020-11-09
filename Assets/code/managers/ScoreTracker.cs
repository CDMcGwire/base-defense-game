using combat.components;
using data.refvalues;
using UnityEngine;

namespace managers {
public class ScoreTracker : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private RefValue<long> scoreRef;
	[SerializeField] private RefValue<long> moneyRef;
	[SerializeField] private RefValue<long> killsRef;
#pragma warning restore 0649

	private void Awake() {
		Debug.Assert(scoreRef != null, "There is no value reference for the score tracker.");
	}

	public void HandleMobKilled(Mob data) {
		scoreRef.Value.Current += data.PointsPerKill;
		moneyRef.Value.Current += data.MoneyPerKill;
		killsRef.Value.Current++;
	}
}
}
