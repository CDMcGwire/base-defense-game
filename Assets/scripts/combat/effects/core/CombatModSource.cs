﻿using System;
using System.Collections.Generic;

using UnityEngine;

namespace combat.effects.core {
/// <summary>
///   Provides additional modifiers to apply to an effect if it applies to an
///   individual hit location on the target.
/// </summary>
[Serializable]
public class CombatModSource : ISerializationCallbackReceiver {
#pragma warning disable 0649
	[SerializeField] private string id = "default-source";
	[SerializeField] private List<PayloadDataSource<CombatMod>> modData;
#pragma warning restore 0649
	
	private readonly Dictionary<string, List<CombatMod>> modsByKind = new();
	private bool initialized = false;

	public string Id => id;

	public IReadOnlyDictionary<string, List<CombatMod>> ModsByKind => modsByKind;

	public void OnBeforeSerialize() { }

	public void OnAfterDeserialize() {
		if (initialized)
			return;
		foreach (var entry in modData) {
			var mod = entry.Value;
			if (mod == null) continue;
			if (!modsByKind.ContainsKey(mod.Kind))
				modsByKind[mod.Kind] = new List<CombatMod>();
			modsByKind[mod.Kind].Add(mod);
		}
		initialized = true;
	}
}
}