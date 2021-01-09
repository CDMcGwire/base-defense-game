using data.refvalues;
using UnityEngine;

namespace data.game {
public class CastleManagementData : MonoBehaviour { 
#pragma warning disable 0649
	[SerializeField] private ReactiveVal<int> health = new ReactiveVal<int>();
	[SerializeField] private ReactiveVal<int> damage = new ReactiveVal<int>();
#pragma warning restore 0649

	public ReactiveVal<int> Health => health;
	public ReactiveVal<int> Damage => damage;
}
}