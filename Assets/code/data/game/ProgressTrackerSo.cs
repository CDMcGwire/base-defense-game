using System;
using UnityEngine;

namespace data.game {
[CreateAssetMenu(fileName = "progress-tracker", menuName = "Game Data/Progress Tracker", order = 0)]
public class ProgressTrackerSo : ScriptableObject {
	// TODO: Build a "Scene Picker" service
	public GamePhase Phase { get; private set; } = GamePhase.PreCombat;
	public long Day { get; private set; }

	public void AdvancePhase() {
		switch (Phase) {
			case GamePhase.PreCombat:
				Phase = GamePhase.Combat;
				Day++;
				break;
			case GamePhase.Combat:
				Phase = GamePhase.PostCombat;
				break;
			case GamePhase.PostCombat:
				Phase = GamePhase.Town;
				break;
			case GamePhase.Town:
				Phase = GamePhase.PreCombat;
				break;
			default: throw new ArgumentOutOfRangeException();
		}
	}

	public void LoadSaveData(ProgressTrackerSaveData saveData) {
		Phase = saveData.phase;
		Day = saveData.day;
	}

	public ProgressTrackerSaveData BuildSaveData()
		=> new ProgressTrackerSaveData {
			phase = Phase,
			day = Day,
		};
}

[Serializable]
public class ProgressTrackerSaveData {
	public GamePhase phase;
	public long day;
}

[Serializable]
public enum GamePhase {
	PreCombat,
	Combat,
	PostCombat,
	Town,
}
}
