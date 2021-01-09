using data.refvalues;
using UnityEngine;

namespace data.game {
public class ProgressTrackerData : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private ReactiveVal<string> scene = new ReactiveVal<string>();
	[SerializeField] private ReactiveVal<GamePhase> phase = new ReactiveVal<GamePhase>();
	[SerializeField] private ReactiveVal<long> day = new ReactiveVal<long>();
#pragma warning restore 0649

	public ReactiveVal<string> Scene => scene;
	public ReactiveVal<GamePhase> Phase => phase;
	public ReactiveVal<long> Day => day;
}
}