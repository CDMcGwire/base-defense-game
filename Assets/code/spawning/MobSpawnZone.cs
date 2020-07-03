using System.Collections.Generic;

using combat.components;

using UnityEngine;

namespace spawning {
public class MobSpawnZone : MonoBehaviour {
	private readonly List<Mob> mobSpawnTray = new List<Mob>();

	private MobSource mobSource;

	private void Start() {
		if (mobSourceData) mobSource = mobSourceData.NewValue(0);
		else enabled = false;
	}

	private void Update() {
		mobSource.NextSpawns(Time.deltaTime, mobSpawnTray);
		foreach (var mob in mobSpawnTray)
			if (mob.Airborne) SpawnAirMob(mob);
			else SpawnGroundMob(mob);
	}

	private Mob SpawnAirMob(Mob prefab) {
		return Instantiate(prefab, NextSpawn(airSpawn.bounds), Quaternion.identity);
	}

	private Mob SpawnGroundMob(Mob prefab) {
		return Instantiate(prefab, NextSpawn(groundSpawn.bounds), Quaternion.identity);
	}

	private static Vector2 NextSpawn(Bounds bounds) {
		return new Vector2(
			Random.Range(bounds.min.x, bounds.max.x),
			Random.Range(bounds.min.y, bounds.max.y)
		);
	}
#pragma warning disable 0649
	[SerializeField] private BoxCollider2D groundSpawn;
	[SerializeField] private BoxCollider2D airSpawn;
	[SerializeField] private MobSourceData mobSourceData;
#pragma warning restore 0649
}
}