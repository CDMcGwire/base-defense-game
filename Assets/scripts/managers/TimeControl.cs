using JetBrains.Annotations;

using UnityEngine;

namespace managers {
public class TimeControl : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private float fastForwardMult = 2.0f;
#pragma warning restore 0649

	[UsedImplicitly]
	public void Pause(bool controlValue) {
		if (!controlValue) return;
		Time.timeScale = 0;
	}

	[UsedImplicitly]
	public void Play(bool controlValue) {
		if (!controlValue) return;
		Time.timeScale = 1;
	}

	[UsedImplicitly]
	public void FastForward(bool controlValue) {
		if (!controlValue) return;
		Time.timeScale = fastForwardMult;
	}
}
}