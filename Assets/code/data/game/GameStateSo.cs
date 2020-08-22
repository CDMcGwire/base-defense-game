using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using refvalues;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace data.game {
[CreateAssetMenu(fileName = "game-state", menuName = "Game Data/State", order = 0), Serializable]
public class GameStateSo : ScriptableObject {
	private const string Save_Ending = ".save";

#pragma warning disable 0649
	[SerializeField] private string saveDir = "game-saves";
	[SerializeField] private StatisticsSo stats;
	[SerializeField] private ProgressTrackerSo progression;
	[SerializeField] private PlayerLoadoutSo playerLoadout;
#pragma warning restore 0649

	public bool Loading => currentLoadOp != null;
	public float LoadProgress => Loading ? currentLoadOp.progress : 1;

	public event Action OnNextLevelLoad;
	
	private AsyncOperation currentLoadOp;
	
	private string FullSaveDir => Path.Combine(Application.persistentDataPath, saveDir);

	public void NextScene() {
		// TODO: Make this get a real value.
		currentLoadOp = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
		currentLoadOp.completed += asyncOp => {
			OnNextLevelLoad?.Invoke();
			OnNextLevelLoad = null;
		};
	}

	public IReadOnlyList<SaveFile> ListSaves() {
		var entries = new List<SaveFile>();
		foreach (var filename in Directory.EnumerateFiles(FullSaveDir)) {
			var fileInfo = new FileInfo(filename);
			var saveName = fileInfo.Name.Replace(Save_Ending, "");
			var saveEntry = new SaveFile(saveName, filename, fileInfo.LastWriteTime);
			entries.Add(saveEntry);
		}
		return entries;
	}

	public async Task LoadGame(string filepath) {
		Debug.LogFormat("Loading {0}", filepath);
		SaveFileData save;
		using (var reader = File.OpenText(filepath)) {
			var content = await reader.ReadToEndAsync();
			save = JsonUtility.FromJson<SaveFileData>(content);
		}
		// Push deserialized data out to managing systems.
		stats.Session.LoadSerializedStats(save.sessionStatsData);
		progression.LoadSaveData(save.progressionData);
		playerLoadout.Data = save.playerLoadoutData;
	}

	public void SaveGame(string saveName) {
		// Pull and collect distributed game state values
		var save = new SaveFileData {
			sessionStatsData = stats.Session.BuildSerializableStats(),
			progressionData = progression.BuildSaveData(),
			playerLoadoutData = playerLoadout.Data,
		};
		var dir = FullSaveDir;
		if (!Directory.Exists(dir))
			Directory.CreateDirectory(dir);
		var filepath = Path.Combine(dir, saveName + Save_Ending);

		File.WriteAllText(filepath, JsonUtility.ToJson(save));
		Debug.LogFormat("Saved to {0}", filepath);
	}
}

[Serializable]
public readonly struct SaveFile {
	public readonly string name;
	public readonly string location;
	public readonly DateTime timestamp;

	public SaveFile(string name, string location, DateTime timestamp) {
		this.name = name;
		this.location = location;
		this.timestamp = timestamp;
	}
}

[Serializable]
public struct SaveFileData {
	public List<SerialStatEntry> sessionStatsData;
	public ProgressTrackerSaveData progressionData;
	public PlayerLoadoutData playerLoadoutData;
}
}

// TODO: !! Clean and simplify. Remove explicit "init" step from the RefValueMaps (follow the custom initialization example on Unity).
