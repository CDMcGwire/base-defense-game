using System.Collections.Generic;
using UnityEngine;

namespace combat.effects.core {
public class CombatEffectPool : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] private EffectSourceComponent baseEffectSource;
	[SerializeField] private ModSourceComponent baseModSource;
	[SerializeField] private float minRecalculateTime = 1f;
	#pragma warning restore 0649
	
	// These collections are reused every time the effect chain is recalculated to reduce allocation costs.
	// Am I pre-optimizing too much? Maybe. But I've always been told allocating collections is expensive af
	// and this thing is already looking like a computer vision algorithm.
	private readonly Dictionary<string, CombatEffectSource> effectSources = new Dictionary<string, CombatEffectSource>();
	private readonly Dictionary<string, CombatModSource> modSources = new Dictionary<string, CombatModSource>();

	private readonly List<CombatEffect> effectChain = new List<CombatEffect>();
	private readonly ModsByKind modChainsByKind = new ModsByKind();

	// These variables are for limiting how frequently this rather expensive calculation process can occur.
	// When the list of effect and mod sources is modified, the flag is set to trigger the compilation on LateUpdate, once
	// All modifications for the frame have been collected.
	private bool modifiedThisFrame = true;
	private float recalculateTimer = 0;

	public IReadOnlyList<CombatEffect> Payload => effectChain;

	private void Awake() {
		if (baseEffectSource != null) effectSources[baseEffectSource.Source.Id] = baseEffectSource.Source;
		if (baseModSource != null) modSources[baseModSource.Source.Id] = baseModSource.Source;
	}

	private void LateUpdate() {
		// TODO: Technically this means the pool is defunct for one frame at first initialization. So should have something to block it from being used in that frame.
		// TODO: -- I Determine it no longer matters but leaving this comment here for the record of it.
		if (recalculateTimer > 0) recalculateTimer -= Time.deltaTime;
		if (!modifiedThisFrame || recalculateTimer > 0) return;
		CompilePayload();
		modifiedThisFrame = false;
	}

	public void AddSource(CombatEffectSource source) {
		if (effectSources.ContainsKey(source.Id)) {
			Debug.LogWarning($"Tried to add existing source {source.Id} to Pool {name}");
			return;
		}
		effectSources[source.Id] = source;
		modifiedThisFrame = true;
	}

	public void AddSource(CombatModSource source) {
		if (modSources.ContainsKey(source.Id)) {
			Debug.LogWarning($"Tried to add existing source {source.Id} to Pool {name}");
			return;
		}
		modSources[source.Id] = source;
		modifiedThisFrame = true;
	}

	public void RemoveSource(CombatEffectSource source) {
		if (effectSources.Remove(source.Id)) modifiedThisFrame = true;
	}

	public void RemoveSource(CombatModSource source) {
		if (modSources.Remove(source.Id)) modifiedThisFrame = true;
	}

	private void CompilePayload() {
		PopulateEffectChain();
		PopulateModChainsByKind();
		ApplyMods();
		recalculateTimer = minRecalculateTime;
	}

	private void PopulateEffectChain() {
		var effects = new List<CombatEffect>();
		foreach (var source in effectSources.Values) effects.AddRange(source.Effects);
		PayloadCompiler.Build(effects, effectChain);
	}

	private void PopulateModChainsByKind() {
		var collectedModsOfKind = new Dictionary<string, List<CombatMod>>();

		// Collect mods from all sources
		foreach (var source in modSources.Values)
		foreach (var entry in source.ModsByKind) {
			if (!collectedModsOfKind.ContainsKey(entry.Key))
				collectedModsOfKind[entry.Key] = new List<CombatMod>();
			collectedModsOfKind[entry.Key].AddRange(entry.Value);
		}

		// Update mod chains of collected kinds
		PayloadCompiler.BuildModChains(collectedModsOfKind, modChainsByKind);
	}

	private void ApplyMods() {
		// Apply global mods if any
		var globalMods = modChainsByKind.Global;
		if (globalMods.Count > 0)
			for (var i = 0; i < effectChain.Count; i++)
				effectChain[i] = effectChain[i].AddMods(globalMods);

		// Grab mods of like kind, if any, and apply
		for (var i = 0; i < effectChain.Count; i++) {
			var modsOfKind = modChainsByKind.OfKind(effectChain[i].Kind);
			if (modsOfKind.Count > 0) effectChain[i] = effectChain[i].AddMods(modsOfKind);
		}
	}
}
}