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
	private const string Temp_Ext = ".partial";
	private const string Save_Ext = ".save";
	private const string Section_Start = ">>>";
	private const string Section_End = "<<<";

#pragma warning disable 0649
	[SerializeField] private string saveDir = "game-saves";
	[SerializeField] private List<DataService> dataServices;
#pragma warning restore 0649

	private string FullSaveDir => Path.Combine(Application.persistentDataPath, saveDir);

	private void OnValidate() {
		foreach (var service in dataServices)
			if (!(service is IPersistableService))
				Debug.LogWarning($"SaveManager [{name}] has a non-serializable service listed as data [{service}].");
	}

	public IReadOnlyList<SaveFile> ListSaves() {
		if (!Directory.Exists(FullSaveDir))
			return new SaveFile[0];
		var entries = new List<SaveFile>();
		foreach (var filename in Directory.EnumerateFiles(FullSaveDir)) {
			var fileInfo = new FileInfo(filename);
			var saveName = fileInfo.Name.Replace(Save_Ext, "");
			var saveEntry = new SaveFile(saveName, filename, fileInfo.LastWriteTime);
			entries.Add(saveEntry);
		}
		return entries;
	}

	public void NewGame() {
		foreach (var service in dataServices)
			service.Initialize();
	}

	public async Task LoadGame(string filepath) {
		Debug.LogFormat("Loading {0}", filepath);

		var sections = new Dictionary<string, IPersistableService>(dataServices.Count);
		foreach (var service in dataServices)
			if (service is IPersistableService persistable)
				sections[service.name] = persistable;

		using var reader = File.OpenText(filepath);
		string line;
		var sectionReader = new SectionReader(reader, Section_End);
		
		while ((line = await reader.ReadLineAsync()) != null) {
			if (!line.StartsWith(Section_Start)) continue;
			var sectionName = line.Substring(Section_Start.Length).Trim();
			if (!sections.ContainsKey(sectionName)) continue;
			
			// See SectionReader class for explanation
			sectionReader.Reset();
			await sections[sectionName].ReadLines(sectionReader);
		}
	}

	public async void SaveGame(string saveName) {
		// Pull and collect distributed game state values
		var dir = FullSaveDir;
		if (!Directory.Exists(dir))
			Directory.CreateDirectory(dir);

		var partFilepath = Path.Combine(dir, saveName + Temp_Ext);
		using (var writer = File.CreateText(partFilepath)) {
			foreach (var service in dataServices) {
				if (!(service is IPersistableService persistable)) continue;
				await writer.WriteLineAsync(Section_Start + service.name);
				await persistable.WriteLines(writer);
				await writer.WriteLineAsync(Section_End);
			}
		}
		var saveFilepath = Path.Combine(dir, saveName + Save_Ext);
		if (File.Exists(saveFilepath))
			File.Delete(saveFilepath);
		File.Move(partFilepath, saveFilepath);
		
		Debug.Log($"Saved game to file [{saveFilepath}].");
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
}