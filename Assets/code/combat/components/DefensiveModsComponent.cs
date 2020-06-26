using System;
using System.Collections.Generic;
using combat.effects.core;
using UnityEngine;

namespace combat.components {
public class DefensiveModsComponent : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private CombatModSource baseDefenseModSource;
	[SerializeField] private List<HitLocation> locationalMods;
#pragma warning restore 0649

	private readonly Dictionary<int, ModsByKind> locationalModSources = new Dictionary<int, ModsByKind>();

	public ModsByKind BaseMods { get; } = new ModsByKind();

	public ModsByKind ModsAtLocation(int locationId)
		=> locationalModSources.ContainsKey(locationId)
			? locationalModSources[locationId]
			: new ModsByKind();

	private void Awake() {
		PayloadCompiler.BuildModChains(baseDefenseModSource.ModsByKind, BaseMods);

		foreach (var location in locationalMods) {
			var modChains = new ModsByKind();
			PayloadCompiler.BuildModChains(location.ModSource.ModsByKind, modChains);
			foreach (var locationCollider in location.Colliders)
				locationalModSources[locationCollider.GetInstanceID()] = modChains;
		}
	}
}

[Serializable]
public struct HitLocation {
#pragma warning disable 0649
	[SerializeField] private CombatModSource modSource;
	[SerializeField] private List<Collider2D> colliders;
#pragma warning disable 0649

	public CombatModSource ModSource => modSource;
	public IReadOnlyList<Collider2D> Colliders => colliders;
}
}