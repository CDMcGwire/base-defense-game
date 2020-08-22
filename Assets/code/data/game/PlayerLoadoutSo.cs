using System;
using UnityEngine;

namespace data.game {
[CreateAssetMenu(fileName = "player-loadout", menuName = "Game Data/Player Loadout", order = 0)]
public class PlayerLoadoutSo : ScriptableObject {
#pragma warning disable 0649
	[SerializeField] private PlayerLoadoutData data;
#pragma warning restore 0649

	public PlayerLoadoutData Data {
		get => data;
		set => data = value ?? new PlayerLoadoutData();
	}
}

[Serializable] public class PlayerLoadoutData { }
}
