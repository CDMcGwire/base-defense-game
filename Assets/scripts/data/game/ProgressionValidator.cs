using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace data.game {
public class ProgressionValidator : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private ProgressTrackerService progressTracker;
	[SerializeField] private GamePhase assignedPhase;
	[SerializeField] private UnityEvent onValid;
#pragma warning restore 0649

	[UsedImplicitly]
	public void Validate() {
		if (progressTracker.Phase.Current == assignedPhase)
			onValid.Invoke();
	}
}
}