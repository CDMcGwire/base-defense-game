using data.game;
using UnityEngine;

namespace combat.managers {
public class PlayerSpawner : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private Camera playerCamera;
	[SerializeField] private PlayerLoadoutService playerLoadout;
	[SerializeField] private Transform spawnPoint;
#pragma warning restore 0649

	private void Start() {
		var playerCharacter = Instantiate(playerLoadout.PlayerCharacter.Current);
		playerCharacter.Initialize(playerLoadout, playerCamera);
		playerCharacter.transform.position = spawnPoint.position;
	}
}
}