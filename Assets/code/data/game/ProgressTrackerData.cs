using data.reactive;
using UnityEngine;

namespace data.game {
public class ProgressTrackerData : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private RxVal<string> scene = new RxVal<string>();
	[SerializeField] private RxVal<GamePhase> phase = new RxVal<GamePhase>();
	[SerializeField] private RxVal<long> day = new RxVal<long>();
#pragma warning restore 0649

	public RxVal<string> Scene => scene;
	public RxVal<GamePhase> Phase => phase;
	public RxVal<long> Day => day;
}
}