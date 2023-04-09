using System;
using System.Collections.Generic;
using UnityEngine;

namespace utility.generics {
[Serializable]
public class DicSerial<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
#pragma warning disable 0649
	[SerializeField] private List<SerialPair<TKey, TValue>> serialData;
#pragma warning restore 0649

	private readonly List<SerialPair<TKey, TValue>> invalidEntries = new(0);

	public void OnBeforeSerialize() {
		serialData.Clear();
		foreach (var entry in this)
			serialData.Add(
				new SerialPair<TKey, TValue> {key = entry.Key, value = entry.Value}
			);
		if (invalidEntries != null)
			serialData.AddRange(invalidEntries);
	}

	public void OnAfterDeserialize() {
		Clear();
		foreach (var entry in serialData)
			if (entry.key == null || ContainsKey(entry.key))
				invalidEntries.Add(entry);
			else
				this[entry.key] = entry.value;
	}
}

[Serializable]
public struct SerialPair<TKey, TValue> {
	public TKey key;
	public TValue value;
}
}