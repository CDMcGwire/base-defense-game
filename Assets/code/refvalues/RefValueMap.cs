using System;
using System.Collections.Generic;
using UnityEngine;

namespace refvalues {
[Serializable]
public class RefValueMap : ISerializationCallbackReceiver {
#pragma warning disable 0649
	[SerializeField] private List<StatEntry> storedValues;
#pragma warning restore 0649

	private readonly Dictionary<string, RefValue> statMap = new Dictionary<string, RefValue>();

	public void OnBeforeSerialize() {
		if (storedValues == null)
			storedValues = new List<StatEntry>();
		storedValues.Clear();
		foreach (var entry in statMap) {
			storedValues.Add(new StatEntry(entry.Key, entry.Value));
		}
	}

	public void OnAfterDeserialize() {
		statMap.Clear();
		PopulateMap();
	}

	private void PopulateMap() {
		var count = 0;
		foreach (var entry in storedValues) {
			if (string.IsNullOrWhiteSpace(entry.key) || statMap.ContainsKey(entry.key))
				statMap[$"Value-{count:000000}"] = entry.value;
			else statMap[entry.key] = entry.value;
			count++;
		}
	}

	public T GetValue<T>(string key) {
		if (statMap.ContainsKey(key)
		    && statMap[key] is RefValue<T> stat) {
			return stat.Value;
		}
		throw new RefValueNotMappedException(
			$"RefValue {key} of type {typeof(T)} has not been configured in the referenced map."
		);
	}

	public void SetValue<T>(string key, T value) {
		if (statMap.ContainsKey(key)
		    && statMap[key] is RefValue<T> stat) {
			stat.Value = value;
		}
		throw new RefValueNotMappedException(
			$"RefValue {key} of type {typeof(T)} has not been configured in the referenced map."
		);
	}

	public bool HasValue<T>(string key)
		=> statMap.ContainsKey(key) && statMap[key] is RefValue<T>;

	public List<SerialStatEntry> BuildSerializableStats() {
		var serializableStats = new List<SerialStatEntry>(statMap.Count);
		foreach (var entry in statMap) {
			var keyValuePair = new SerialStatEntry(entry.Key, entry.Value.SerializeCurrentValue());
			serializableStats.Add(keyValuePair);
			Debug.Log($"Will serialize {JsonUtility.ToJson(keyValuePair)}");
		}
		return serializableStats;
	}

	public void LoadSerializedStats(IEnumerable<SerialStatEntry> statData) {
		foreach (var entry in statData) {
			if (statMap.ContainsKey(entry.key))
				statMap[entry.key].LoadFromJson(entry.value);
		}
	}
}

[Serializable]
public struct StatEntry {
	public string key;
	public RefValue value;
	
	public StatEntry(string key, RefValue value) {
		this.key = key;
		this.value = value;
	}
}

[Serializable]
public struct SerialStatEntry {
	public string key;
	public string value;
	
	public SerialStatEntry(string key, string value) {
		this.key = key;
		this.value = value;
	}
}

public class RefValueNotMappedException : Exception {
	public RefValueNotMappedException(string message) : base(message) { }
}
}
