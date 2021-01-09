using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using data.refvalues;
using data.service;
using UnityEngine;

namespace data.game {
[CreateAssetMenu(fileName = "statistics-service", menuName = "Service/Statistics", order = 0)]
public class GameStatsService : DataService, IPersistableService {
#pragma warning disable 0649
	[SerializeField] private List<RefValue> stats;
#pragma warning restore 0649

	public IReadOnlyList<RefValue> Stats => stats;

	public async Task WriteLines(StreamWriter stream) {
		foreach (var stat in stats)
			await stream.WriteLineAsync($"{stat.name}={stat.AsText()}");
	}

	public async Task ReadLines(SectionReader stream) {
		var statsByName = new Dictionary<string, RefValue>(stats.Count);
		foreach (var stat in stats)
			statsByName[stat.name] = stat;
		string line;
		while ((line = await stream.ReadLineAsync()) != null) {
			var pair = line.Split('=');
			if (pair.Length < 2) continue;
			if (statsByName.ContainsKey(pair[0]))
				statsByName[pair[0]].Coerce(pair[1]);
		}
	}

	public override void Initialize() {
		foreach (var stat in stats)
			stat.Initialize();
	}
}
}