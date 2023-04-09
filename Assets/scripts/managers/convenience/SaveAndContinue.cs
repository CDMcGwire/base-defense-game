using data.game;
using JetBrains.Annotations;
using UnityEngine;

namespace managers.convenience {
public class SaveAndContinue : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private SaveManagerService saveManager;
	[SerializeField] private ProgressTrackerService progressTracker;
#pragma warning restore 0649

	[UsedImplicitly]
	public void Continue() {
		saveManager.AutoSave();
		progressTracker.StartNextScene();
	}
}
}