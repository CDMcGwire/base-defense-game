using System;
using System.Collections.Generic;
using combat.components;
using UnityEngine;
using Random = UnityEngine.Random;

namespace spawning {
[CreateAssetMenu(fileName = "mob-source", menuName = "Combat/Enemy/Mob Source", order = 0)]
public class MobSourceData : ScriptableObject, ISerializationCallbackReceiver {
#pragma warning disable 0649
	[SerializeField] private List<PeriodicSpawnEntry> periodicSpawnEntries = new List<PeriodicSpawnEntry>();
	[SerializeField] private List<TimedSpawn> scriptedSpawnEntries = new List<TimedSpawn>();
#pragma warning restore 0649

	private readonly SortedList<float, TimedSpawn> sortedScriptedSpawns = new SortedList<float, TimedSpawn>();

	public MobSource NewValue(float startTime)
		=> new MobSource(startTime, periodicSpawnEntries, sortedScriptedSpawns);

	public void OnBeforeSerialize() { }

	public void OnAfterDeserialize() {
		foreach (var entry in scriptedSpawnEntries)
			sortedScriptedSpawns.Add(entry.Time, entry);
	}
}

public class MobSource {
	private float time;
	private readonly IReadOnlyList<PeriodicSpawnEntry> periodicSpawnEntries;
	private readonly List<TimedSpawn> nextPeriodicSpawns;
	private readonly Queue<TimedSpawn> scriptedSpawnQueue;

	public MobSource(
		float startTime,
		IReadOnlyList<PeriodicSpawnEntry> periodicSpawnEntries,
		SortedList<float, TimedSpawn> scriptedSpawnEntries
	) {
		time = startTime;
		this.periodicSpawnEntries = periodicSpawnEntries;
		nextPeriodicSpawns = new List<TimedSpawn>(periodicSpawnEntries.Count);
		// Scripted spawns should be sorted by time-ascending when passed to constructor
		scriptedSpawnQueue = new Queue<TimedSpawn>(scriptedSpawnEntries.Values);

		foreach (var entry in periodicSpawnEntries)
			nextPeriodicSpawns.Add(
				entry.InitialDelay > 0
					? new TimedSpawn(null, entry.InitialDelay)
					: entry.Next(time)
			);
	}

	public void NextSpawns(float deltaTime, List<Mob> output) {
		time += deltaTime;
		output.Clear();

		// Check for periodic spawns that are ready and refresh
		for (var i = 0; i < nextPeriodicSpawns.Count; i++) {
			var entry = nextPeriodicSpawns[i];
			if (!(time >= entry.Time)) continue;
			if (ReferenceEquals(entry.Mob, null)) {
				nextPeriodicSpawns[i] = new TimedSpawn(periodicSpawnEntries[i].Mob, 0);
			}
			else {
				output.Add(entry.Mob);
				nextPeriodicSpawns[i] = periodicSpawnEntries[i].Next(time);
			}
		}
		// Collect scripted spawns
		while (ShouldSpawnNextScripted)
			output.Add(scriptedSpawnQueue.Dequeue().Mob);
	}

	private bool ShouldSpawnNextScripted
		=> scriptedSpawnQueue.Count > 0 && time >= scriptedSpawnQueue.Peek().Time;
}

[Serializable]
public struct PeriodicSpawnEntry {
#pragma warning disable 0649
	[SerializeField] private Mob mob;
	[SerializeField] private float basePeriod;
	[SerializeField] private float relativeVariation;
	[SerializeField] private float absoluteVariation;
	[SerializeField] private float initialDelay;
#pragma warning restore 0649

	public Mob Mob => mob;
	public float BasePeriod => basePeriod;
	public float RelativeVariation => relativeVariation;
	public float AbsoluteVariation => absoluteVariation;
	public float InitialDelay => initialDelay;

	public TimedSpawn Next(float currentTime) {
		var variation = basePeriod * Math.Min(0, relativeVariation) + Math.Min(0, absoluteVariation);
		var finalPeriod = basePeriod + Random.Range(-variation, variation);
		return new TimedSpawn(mob, currentTime + finalPeriod);
	}
}

[Serializable]
public struct TimedSpawn {
#pragma warning disable 0649
	[SerializeField] private Mob mob;
	[SerializeField] private float time;
#pragma warning restore 0649

	public Mob Mob => mob;
	public float Time => time;

	public TimedSpawn(Mob mob, float time) {
		this.mob = mob;
		this.time = time;
	}
}
}