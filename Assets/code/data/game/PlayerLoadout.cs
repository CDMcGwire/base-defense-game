using UnityEngine;

namespace data.game {
public class PlayerLoadout : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private PlayerLoadoutData loadoutData;
#pragma warning restore 0649

	public PlayerLoadoutData LoadoutData {
		get => loadoutData;
		set => loadoutData = value;
	}
}
}