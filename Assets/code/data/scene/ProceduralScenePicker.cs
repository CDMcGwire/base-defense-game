using System;
using System.Collections.Generic;
using data.condition;
using data.game;
using data.refconst;
using UnityEngine;
using Random = UnityEngine.Random;

namespace data.scene {
/// <summary>
/// A scene picker which procedurally chooses from a pool of available scenes
/// using the current loaded game progress.
/// </summary>
[CreateAssetMenu(fileName = "procedural-scene-picker", menuName = "Scene Picker/Procedural", order = 0)]
public class ProceduralScenePicker : ScenePicker {
#pragma warning disable 0649
	[SerializeField] private ProgressTrackerService progressTracker;
	[SerializeField] private List<ProceduralScene> scenePool;
#pragma warning restore 0649

	private readonly SortedList<long, ProceduralScene> sortedScenes = new SortedList<long, ProceduralScene>();

	// TODO: This isn't reliable enough. Different environments treat it differently.
	private void OnEnable() => SortScenes();

	/// <summary>
	/// Determine the set of available scenes and pick one by a weighted random.
	/// </summary>
	/// <returns>The name of the scene to load.</returns>
	public override string Next() {
		// Get all scenes available for the current day.
		var validScenes = new List<ProceduralScene>();
		var totalWeight = 0;
		foreach (var entry in sortedScenes) {
			if (entry.Key > progressTracker.Day) break;
			if (entry.Value.ToDay < progressTracker.Day
			    || !entry.Value.ConditionsMet()) continue;
			validScenes.Add(entry.Value);
			totalWeight += entry.Value.Weight;
		}
		// Of the remaining scenes pick a weighted random.
		var target = Random.Range(0, totalWeight);
		foreach (var scene in validScenes) {
			target -= scene.Weight;
			if (target < 0) return scene.Name.Trim();
		}
		// If no scene is available, return an empty string. (Nothing will be loaded)
		return "";
	}

	/// <summary>
	/// Initializes the list of usable scenes, sorting by the first day it is
	/// available.
	/// </summary>
	private void SortScenes() {
		sortedScenes.Clear();
		foreach (var scene in scenePool)
			sortedScenes.Add(scene.FromDay, scene);
	}
}

/// <summary>
/// Stores the parameters for a procedurally selectable scene.
/// </summary>
[Serializable]
public struct ProceduralScene : ISerializationCallbackReceiver {
#pragma warning disable 0649
	[SerializeField] private RefConst<string> scene;
	[SerializeField] private int fromDay;
	[SerializeField] private int toDay;
	[SerializeField] private int weight;
	[SerializeField] private List<DataCondition> conditions;
#pragma warning restore 0649

	/// <summary>
	/// The name of the scene as used by Unity's Scene Manager.
	/// </summary>
	public string Name => scene.Value;

	/// <summary>
	/// The first in-game day this scene will be available to pick.
	/// </summary>
	public int FromDay => fromDay;

	/// <summary>
	/// The last in-game day this scene will be available to pick.
	/// </summary>
	public int ToDay => toDay;

	/// <summary>
	/// The "weight" of this entry for random selection.
	/// </summary>
	public int Weight => weight;

	/// <summary>
	/// The additional, specific conditions that must be met for this scene.
	/// </summary>
	public List<DataCondition> Conditions => conditions;

	public void OnBeforeSerialize() { }

	public void OnAfterDeserialize() {
		if (fromDay < 0) fromDay = 0;
		if (toDay < fromDay) toDay = fromDay;
		if (weight < 1) weight = 1;
	}

	/// <summary>
	/// Helper method to check through all the conditions on the scene.
	/// </summary>
	/// <returns>True if all conditions are met, else false.</returns>
	public bool ConditionsMet() {
		foreach (var condition in conditions) {
			if (!condition.Evaluate()) return false;
		}
		return true;
	}
}
}