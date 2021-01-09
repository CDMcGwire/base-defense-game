using System;
using System.Collections.Generic;
using combat.components;
using data.game;
using UnityEngine;
using utility.math;
using Random = UnityEngine.Random;

namespace spawning {
[CreateAssetMenu(fileName = "mob-source", menuName = "Combat/Enemy/Mob Source", order = 0)]
public class MobSourceData : ScriptableObject, ISerializationCallbackReceiver {
#pragma warning disable 0649
	[SerializeField] private ProgressTrackerService progressTracker;
	[SerializeField] private List<ProceduralSpawnGenerator> spawnGenerators = new List<ProceduralSpawnGenerator>();
	[SerializeField] private List<TimedSpawn> scriptedSpawnEntries = new List<TimedSpawn>();
#pragma warning restore 0649

	private readonly SortedList<float, TimedSpawn> sortedScriptedSpawns = new SortedList<float, TimedSpawn>();

	public void OnBeforeSerialize() { }

	public void OnAfterDeserialize() {
		foreach (var entry in scriptedSpawnEntries)
			sortedScriptedSpawns.Add(entry.Time, entry);
	}

	public MobSource NewValue(float startTime) {
		var periodicSpawns = new List<PeriodicSpawn>();
		var day = progressTracker.Day.Current;
		foreach (var generator in spawnGenerators)
			if (generator.CanSpawn(day))
				periodicSpawns.Add(generator.CalculatePeriodicSpawn(day));
		return new MobSource(
			startTime,
			periodicSpawns,
			sortedScriptedSpawns
		); // TODO: Setup the new scriptable object and check if this works.
	}
	// TODO: Also, there is no continue button on the town scene so fix that and make sure it saves.
}

public class MobSource {
	private readonly List<TimedSpawn> nextPeriodicSpawns;
	private readonly IReadOnlyList<PeriodicSpawn> periodicSpawnEntries;
	private readonly Queue<TimedSpawn> scriptedSpawnQueue;
	private float time;

	public MobSource(
		float startTime,
		IReadOnlyList<PeriodicSpawn> periodicSpawnEntries,
		SortedList<float, TimedSpawn> scriptedSpawnEntries
	) {
		time = startTime;
		this.periodicSpawnEntries = periodicSpawnEntries;
		nextPeriodicSpawns = new List<TimedSpawn>(periodicSpawnEntries.Count);
		// Scripted spawns should be sorted by time-ascending when passed to constructor
		scriptedSpawnQueue = new Queue<TimedSpawn>(scriptedSpawnEntries.Values);

		foreach (var entry in periodicSpawnEntries)
			nextPeriodicSpawns.Add(entry.Next(time, entry.InitialDelay));
	}

	private bool ShouldSpawnNextScripted
		=> scriptedSpawnQueue.Count > 0
		   && time >= scriptedSpawnQueue.Peek().Time;

	public void NextSpawns(float deltaTime, List<Mob> output) {
		time += deltaTime;
		output.Clear();

		// Check for periodic spawns that are ready and refresh
		for (var i = 0; i < nextPeriodicSpawns.Count; i++) {
			var entry = nextPeriodicSpawns[i];
			if (!(time >= entry.Time)) continue;
			// If it was a delay entry, discard and create a new one.
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
}

[Serializable]
public struct PeriodicSpawn {
#pragma warning disable 0649
	[SerializeField] private Mob mob;
	[SerializeField] private float interval;
	[SerializeField] private float relativeVariation;
	[SerializeField] private float absoluteVariation;
	[SerializeField] private float initialDelay;
#pragma warning restore 0649

	public Mob Mob => mob;
	public float Interval => interval;
	public float RelativeVariation => relativeVariation;
	public float AbsoluteVariation => absoluteVariation;
	public float InitialDelay => initialDelay;

	public PeriodicSpawn(
		Mob mob,
		float interval,
		float relativeVariation,
		float absoluteVariation,
		float initialDelay
	) {
		this.mob = mob;
		this.interval = interval;
		this.relativeVariation = relativeVariation;
		this.absoluteVariation = absoluteVariation;
		this.initialDelay = initialDelay;
	}

	public TimedSpawn Next(float currentTime, float delay = 0f) {
		var variation = interval * Math.Min(0, relativeVariation) + Math.Min(0, absoluteVariation);
		var finalPeriod = interval + Random.Range(-variation, variation);
		return new TimedSpawn(mob, currentTime + finalPeriod + delay);
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

[Serializable]
public class ProceduralSpawnGenerator {
#pragma warning disable 0649
	[SerializeField] private Mob mob;
	[SerializeField] private int firstDay;
	[SerializeField] private int lastDay;
	[Space(10)]
	[SerializeField] private float baseInterval;
	[SerializeField] private ScalingValue intervalScaling;
	[Space(10)]
	[SerializeField] private float baseRelativeVariation;
	[SerializeField] private ScalingValue relativeVariationScaling;
	[Space(10)]
	[SerializeField] private float baseAbsoluteVariation;
	[SerializeField] private ScalingValue absoluteVariationScaling;
	[Space(10)]
	[SerializeField] private float baseInitialDelay;
	[SerializeField] private ScalingValue initialDelayScaling;
#pragma warning restore 0649

	public bool CanSpawn(long day)
		=> day >= firstDay && (lastDay < 1 || day <= lastDay);

	public PeriodicSpawn CalculatePeriodicSpawn(long day)
		=> new PeriodicSpawn(
			mob,
			intervalScaling.At(baseInterval, day),
			relativeVariationScaling.At(baseRelativeVariation, day),
			absoluteVariationScaling.At(baseAbsoluteVariation, day),
			initialDelayScaling.At(baseInitialDelay, day)
		);
}
}