using System;
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
public class ProgressTrackerService : Service<ProgressTracker> {
	private const string Invalid_Scene = "INVALID";

#pragma warning disable 0649
	[SerializeField] private ScenePicker defenseScenePicker;
	[SerializeField] private ScenePicker townScenePicker;
	[SerializeField] private ScenePicker preDefenseScenePicker;
	[SerializeField] private ScenePicker postDefenseScenePicker;
	[SerializeField] private ScenePicker endGameScenePicker;
	[Space(10)]
	[SerializeField] private SceneLoaderService sceneLoader;
#pragma warning restore 0649

	/// <summary>
	/// The loadable name of the current scene.
	/// </summary>
	public string CurrentScene {
		get => DataComponent.progressData.currentScene;
		private set => DataComponent.progressData.currentScene = value;
	}

	/// <summary>
	/// The current phase of the game.
	/// </summary>
	public GamePhase Phase {
		get => DataComponent.progressData.phase;
		private set => DataComponent.progressData.phase = value;
	}

	/// <summary>
	/// The current day of the game starting from 0.
	/// </summary>
	public long Day {
		get => DataComponent.progressData.day;
		private set => DataComponent.progressData.day = value;
	}

	/// <summary>
	/// For checking if the loaded save was in an invalid state.
	/// </summary>
	public bool ValidScene => CurrentScene != Invalid_Scene;

	public event Action OnNextSceneEnd;

	[UsedImplicitly]
	public void NewGame() {
		Clear();
		StartNextScene();
	}

	public void StartNextScene() {
		// Advance the phase.
		CurrentScene = AdvancePhase();
		// Check if a valid scene name was returned.
		if (string.IsNullOrWhiteSpace(CurrentScene)) {
			switch (Phase) {
				// If no scene was returned for pre/post combat, skip that phase.
				case GamePhase.PreDefense:
				case GamePhase.PostDefense:
					CurrentScene = AdvancePhase();
					break;
			}
		}
		try {
			sceneLoader.LoadScene(CurrentScene);
		}
		catch (SceneLoadException) {
			CurrentScene = Invalid_Scene;
			throw;
		}
		// Fire off any events waiting for the scene to end.
		// TODO: Come back to this if this ends up needing to reference the current scene info.
		OnNextSceneEnd?.Invoke();
		OnNextSceneEnd = null;
	}

	/// <summary>
	/// Method to be called when the current run should end.
	/// </summary>
	public void EndGame() {
		sceneLoader.LoadScene(endGameScenePicker.Next());
	}

	/// <summary>
	/// Creates a serializable data object from the current state.
	/// </summary>
	/// <returns>A serializable data object.</returns>
	public ProgressTrackerData BuildSaveData()
		=> DataComponent.progressData;

	/// <summary>
	/// Sets the current state from the contents of a deserialized save file.
	/// </summary>
	/// <param name="saveData">The deserialized data object.</param>
	public void LoadSaveData(ProgressTrackerData saveData)
		=> DataComponent.progressData = saveData;

	public void Clear() {
		CurrentScene = "";
		Phase = GamePhase.PreDefense;
		Day = 0;
	}

	/// <summary>
	/// Progresses the game session to the next ordered phase and returns the
	/// name of the next scene to load.
	/// </summary>
	/// <returns>The name of the next scene or an empty string if there is none.</returns>
	private string AdvancePhase() {
		switch (Phase) {
			case GamePhase.PreDefense:
				Phase = GamePhase.Defense;
				Day++;
				return defenseScenePicker.Next();
			case GamePhase.Defense:
				Phase = GamePhase.PostDefense;
				return postDefenseScenePicker.Next();
			case GamePhase.PostDefense:
				Phase = GamePhase.Town;
				return townScenePicker.Next();
			case GamePhase.Town:
				Phase = GamePhase.PreDefense;
				return preDefenseScenePicker.Next();
		}
		return "";
	}
}

/// <summary>
/// An object representing all the data which needs to be serialized to a save
/// file.
/// </summary>
[Serializable]
public struct ProgressTrackerData {
	public string currentScene;
	public GamePhase phase;
	public long day;
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