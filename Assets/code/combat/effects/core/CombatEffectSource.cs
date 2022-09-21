using System;
using System.Collections.Generic;

using UnityEngine;

namespace combat.effects.core {
[Serializable]
public class CombatEffectSource : ISerializationCallbackReceiver {
#pragma warning disable 0649
	[SerializeField] private string id = "default-source";
	[SerializeField] private List<PayloadDataSource<CombatEffect>> effectsData = new();
#pragma warning restore 0649
	
	public string Id => id;
	public IReadOnlyList<CombatEffect> Effects { get; private set; }

	public void OnBeforeSerialize() { }

	public void OnAfterDeserialize() {
		var tempEffects = new List<CombatEffect>(effectsData.Count);
		foreach (var effectData in effectsData)
			if (effectData != null)
				tempEffects.Add(effectData.Value);
		Effects = tempEffects;
	}
}
}