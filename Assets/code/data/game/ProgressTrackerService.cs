using System;
using System.IO;
using System.Threading.Tasks;
using data.reactive;
using data.scene;
using data.service;
using JetBrains.Annotations;
using UnityEngine;

namespace data.game {
/// <summary>
/// The main in-game progress tracker. Where progress means the "day" and the
/// "phase" of the day.
/// </summary>
[CreateAssetMenu(fileName = "progress-tracker-service", menuName = "Service/Progress Tracker", order = 0)]
public class ProgressTrackerService : DataService<ProgressTrackerData>, IPersistableService {
	private const string Invalid_Scene = "INVALID";

#pragma warning disable 0649
	[SerializeField] private GamePhase initialPhase = GamePhase.PreDefense;
	[SerializeField] private long initialDay = 0;
	[Space(10)]
	[SerializeField] private ScenePicker defenseScenePicker;
	[SerializeField] private ScenePicker townScenePicker;
	[SerializeField] private ScenePicker preDefenseScenePicker;
	[SerializeField] private ScenePicker postDefenseScenePicker;
	[SerializeField] private ScenePicker endGameScenePicker;
	[Space(10)]
	[SerializeField] private SceneLoaderService sceneLoader;
#pragma warning restore 0649

	public IRxReadonlyVal<long> Day => Data.Day;
	public IRxReadonlyVal<GamePhase> Phase => Data.Phase;
	public IRxReadonlyVal<string> Scene => Data.Scene;

	/// <summary>
	/// For checking if the loaded save was in an invalid state.
	/// </summary>
	public bool ValidScene => Data.Scene.Current != Invalid_Scene;

	public event Action OnNextSceneEnd;

	public override void Initialize() {
		Data.Scene.Current = "";
		Data.Phase.Current = initialPhase;
		Data.Day.Current = initialDay;
	}

	[UsedImplicitly]
	public void NewGame() {
		Initialize();
		// Begin with a pre-defense phase if one is available, else continue to defense.
		if (!BeginPhase())
			StartNextScene();
	}

	public void StartNextScene() {
		AdvancePhase();
		while (!BeginPhase()) {
			switch (Data.Phase.Current) {
				case GamePhase.PreDefense:
				case GamePhase.PostDefense:
					AdvancePhase();
					break;
				default:
					throw new Exception(
						$"Tried to begin a non-optional game phase [{Data.Phase.Current}] but no scene could be chosen."
					);
			}
		}
		OnNextSceneEnd?.Invoke();
		OnNextSceneEnd = null;
	}

	private bool BeginPhase() {
		// Determine the scene to load for the current phase.
		Data.Scene.Current = PickScene();
		// Check if a valid scene name was returned.
		if (string.IsNullOrWhiteSpace(Data.Scene.Current))
			return false;
		try {
			sceneLoader.LoadScene(Data.Scene.Current);
		}
		catch (SceneLoadException) {
			Data.Scene.Current = Invalid_Scene;
			throw;
		}
		return true;
	}

	/// <summary>
	/// Method to be called when the current run should end.
	/// </summary>
	public void EndGame()
		=> sceneLoader.LoadScene(endGameScenePicker.Next());

	/// <summary>
	/// Progresses the game session to the next ordered phase and returns the
	/// name of the next scene to load.
	/// </summary>
	/// <returns>The name of the next scene or an empty string if there is none.</returns>
	private void AdvancePhase() {
		switch (Data.Phase.Current) {
			case GamePhase.PreDefense:
				Data.Phase.Current = GamePhase.Defense;
				Data.Day.Current++;
				break;
			case GamePhase.Defense:
				Data.Phase.Current = GamePhase.PostDefense;
				break;
			case GamePhase.PostDefense:
				Data.Phase.Current = GamePhase.Town;
				break;
			case GamePhase.Town:
				Data.Phase.Current = GamePhase.PreDefense;
				break;
		}
	}

	private string PickScene()
		=> Data.Phase.Current switch {
			GamePhase.PreDefense => preDefenseScenePicker.Next(),
			GamePhase.Defense => defenseScenePicker.Next(),
			GamePhase.PostDefense => postDefenseScenePicker.Next(),
			GamePhase.Town => townScenePicker.Next(),
			_ => "",
		};

	public async Task WriteLines(StreamWriter stream)
		=> await stream.WriteLineAsync(JsonUtility.ToJson(Data));

	public async Task ReadLines(SectionReader stream)
		=> JsonUtility.FromJsonOverwrite(await stream.ReadLineAsync(), Data);
}

/// <summary>
/// Enumerates all of the phases of an in-game "day".
/// </summary>
[Serializable]
public enum GamePhase {
	PreDefense,
	Defense,
	PostDefense,
	Town,
}

public class ProgressTrackerException : Exception {
	public ProgressTrackerException(string message) : base(message) { }
}
}