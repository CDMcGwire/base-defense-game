using System;
using refvalues;
using UnityEngine;

namespace data.game {
[CreateAssetMenu(fileName = "game-statistics", menuName = "Game Data/Game Stats", order = 0)]
public class StatisticsSo : ScriptableObject {
#pragma warning disable 0649
	[SerializeField] private string profileStatsSavePath = "";
	[SerializeField] private RefValueMap session = new RefValueMap();
	[SerializeField] private RefValueMap profile = new RefValueMap();
#pragma warning restore 0649

	public RefValueMap Session {
		get => session;
		set => session = value ?? new RefValueMap();
	}

	public RefValueMap Profile {
		get => profile;
		set => profile = value ?? new RefValueMap();
	}

	public void LoadProfile() {
		// TODO: Load the profile statistics file if present.
	}

	public void SaveProfile() {
		// TODO: Save the current profile statistics to file.
	}
}
}
