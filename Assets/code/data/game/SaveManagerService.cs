using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using data.service;
using UnityEngine;

namespace data.game {
/// <summary>
/// The primary interface for managing save files.
/// </summary>
[CreateAssetMenu(fileName = "save-manager-service", menuName = "Service/Game Save", order = 0), Serializable]
public class SaveManagerService : Service {
	private const string Save_Ending = ".save";

#pragma warning disable 0649
	[SerializeField] private string saveDir = "game-saves";
	[SerializeField] private StatisticsService statistics;
	[SerializeField] private ProgressTrackerService progressTracker;
	[SerializeField] private PlayerLoadoutService playerLoadout;
#pragma warning restore 0649

	private string FullSaveDir => Path.Combine(Application.persistentDataPath, saveDir);

	public IReadOnlyList<SaveFile> ListSaves() {
		if (!Directory.Exists(FullSaveDir))
			return new SaveFile[0];
		var entries = new List<SaveFile>();
		foreach (var filename in Directory.EnumerateFiles(FullSaveDir)) {
			var fileInfo = new FileInfo(filename);
			var saveName = fileInfo.Name.Replace(Save_Ending, "");
			var saveEntry = new SaveFile(saveName, filename, fileInfo.LastWriteTime);
			entries.Add(saveEntry);
		}
		return entries;
	}

	public void NewGame() {
		statistics.Clear();
		progressTracker.Clear();
		playerLoadout.Clear();
	}

	public async Task LoadGame(string filepath) {
		Debug.LogFormat("Loading {0}", filepath);
		SaveFileData save;
		using (var reader = File.OpenText(filepath)) {
			var content = await reader.ReadToEndAsync();
			save = JsonUtility.FromJson<SaveFileData>(content);
		}
		// Push deserialized data out to managing systems.
		statistics.LoadSessionSaveData(save.sessionStatsData);
		progressTracker.LoadSaveData(save.progressionData);
		playerLoadout.LoadFromData(save.playerLoadoutData);
	}

	public void SaveGame(string saveName) {
		// Pull and collect distributed game state values
		var save = new SaveFileData {
			sessionStatsData = statistics.BuildSessionSaveData(),
			progressionData = progressTracker.BuildSaveData(),
			playerLoadoutData = playerLoadout.BuildSaveData(),
		};
		var dir = FullSaveDir;
		if (!Directory.Exists(dir))
			Directory.CreateDirectory(dir);
		var filepath = Path.Combine(dir, saveName + Save_Ending);

		File.WriteAllText(filepath, JsonUtility.ToJson(save));
		Debug.LogFormat("Saved to {0}", filepath);
	}

	public void AutoSave() => SaveGame("AutoSave");
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
	public ProgressTrackerData progressionData;
	public PlayerLoadoutData playerLoadoutData;
}
}
