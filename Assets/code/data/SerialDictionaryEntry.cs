using System;
using System.Collections.Generic;
using UnityEngine;

namespace data {
[Serializable]
public struct SerialDictionaryEntry<TKey, TValue> {
#pragma warning disable 0649
	[SerializeField] private TKey key;
	[SerializeField] private TValue value;
#pragma warning restore 0649

	public TKey Key => key;
	public TValue Value => value;

	public SerialDictionaryEntry(KeyValuePair<TKey, TValue> entry) {
		key = entry.Key;
		value = entry.Value;
	}
}
}