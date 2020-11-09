using System;
using UnityEngine;

namespace data.game {
public class ProgressTracker : MonoBehaviour {
	[NonSerialized] public ProgressTrackerData progressData;
	[NonSerialized] public LoadScreen activeLoadScreen;
}
}