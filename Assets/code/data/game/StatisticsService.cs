using System;
using System.Collections.Generic;
using data.refvalues;
using data.service;
using UnityEngine;

namespace data.game {
[CreateAssetMenu(fileName = "statistics-service", menuName = "Service/Statistics", order = 0)]
public class StatisticsService : Service {
#pragma warning disable 0649
	[SerializeField] private string profileStatsSavePath = "";
	[SerializeField] private List<StatEntry> session;
	[SerializeField] private List<StatEntry> profile;
#pragma warning restore 0649

	public IReadOnlyList<StatEntry> Session => session;
	public IReadOnlyList<StatEntry> Profile => profile;

	public List<SerialStatEntry> BuildSessionSaveData()
		=> BuildSerializableStatList(Session);

	public void LoadSessionSaveData(IReadOnlyList<SerialStatEntry> serialStats)
		=> LoadSerializedStatList(Session, serialStats);

	public List<SerialStatEntry> BuildProfileSaveData()
		=> BuildSerializableStatList(Profile);

	public void LoadProfileSaveData(IReadOnlyList<SerialStatEntry> serialStats)
		=> LoadSerializedStatList(Profile, serialStats);

	public void Clear() {
		foreach (var entry in session)
			entry.RefValue.Reset();
	}

	private static List<SerialStatEntry> BuildSerializableStatList(
		IReadOnlyList<StatEntry> stats
	) {
		var serialStats = new List<SerialStatEntry>(stats.Count);
		foreach (var entry in stats)
			serialStats.Add(new SerialStatEntry(entry));
		return serialStats;
	}

	private static void LoadSerializedStatList(
		IReadOnlyList<StatEntry> stats,
		IReadOnlyList<SerialStatEntry> serialStats
	) {
		var serialValuesByName = new Dictionary<string, string>(serialStats.Count);
		foreach (var entry in serialStats)
			serialValuesByName[entry.DisplayName] = entry.SerializedData;
		foreach (var stat in stats)
			if (serialValuesByName.ContainsKey(stat.DisplayName))
				stat.RefValue.FromJson(serialValuesByName[stat.DisplayName]);
	}
}

[Serializable]
public struct StatEntry {
#pragma warning disable 0649
	[SerializeField] private string displayName;
	[SerializeField] private RefValue refValue;
#pragma warning restore 0649

	public string DisplayName => displayName;
	public RefValue RefValue => refValue;
}

[Serializable]
public struct SerialStatEntry {
#pragma warning disable 0649
	[SerializeField] private string displayName;
	[SerializeField] private string serializedData;
#pragma warning restore 0649

	public string DisplayName => displayName;
	public string SerializedData => serializedData;

	public SerialStatEntry(StatEntry statEntry) {
		displayName = statEntry.DisplayName;
		serializedData = statEntry.RefValue.ToJson();
	}
}
}