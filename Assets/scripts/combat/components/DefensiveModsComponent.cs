using System;
using System.Collections.Generic;

using combat.effects.core;

using UnityEngine;

namespace combat.components {
/// <summary>
///   <param>
///     A component that acts as the central repository of combat mods that should
///     be applied to effects targeting this <see cref="GameObject" />.
///   </param>
///   <para>
///     Locational modifiers are mapped based on the component instance id of the
///     associated <see cref="Collider2D" />.
///   </para>
/// </summary>
public class DefensiveModsComponent : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private CombatModSource baseDefenseModSource;
	[SerializeField] private List<HitLocation> locationalMods;
#pragma warning restore 0649

	private readonly Dictionary<int, ModsByKind> locationalModSources = new();

	/// <summary>
	///   The set of modifiers that apply regardless of specific location
	///   targeted.
	/// </summary>
	public ModsByKind BaseMods { get; } = new();

	/// <summary>The set of modifiers that apply to the given component instance id.</summary>
	/// <param name="locationId">
	///   The component instance id of the
	///   <see cref="Collider2D" /> targeted.
	/// </param>
	/// <returns>
	///   A <see cref="ModsByKind" /> collection for the given location, or an empty
	///   one if the location is not tracked.
	/// </returns>
	public ModsByKind ModsAtLocation(int locationId) {
		return locationalModSources.ContainsKey(locationId)
			? locationalModSources[locationId]
			: new ModsByKind();
	}

	private void Awake() {
		// On Awake, the serialized list of hit locations is queried for mods and compiled into a map.
		PayloadCompiler.BuildModChains(baseDefenseModSource.ModsByKind, BaseMods);

		foreach (var location in locationalMods) {
			var modChains = new ModsByKind();
			PayloadCompiler.BuildModChains(location.ModSource.ModsByKind, modChains);
			foreach (var locationCollider in location.Colliders)
				locationalModSources[locationCollider.GetInstanceID()] = modChains;
		}
	}
}

/// <summary>
///   A struct for serializing a <see cref="CombatModSource" /> and it's associated
///   <see cref="Collider2D" /> list.
/// </summary>
[Serializable]
public struct HitLocation {
#pragma warning disable 0649
	[SerializeField] private CombatModSource modSource;
	[SerializeField] private List<Collider2D> colliders;
#pragma warning disable 0649

	/// <summary>The associated <see cref="ModSource" />.</summary>
	public CombatModSource ModSource => modSource;
	/// <summary>
	///   The <see cref="Collider2D" /> list that should map to the
	///   <see cref="ModSource" /> when targeted.
	/// </summary>
	public IReadOnlyList<Collider2D> Colliders => colliders;
}
}