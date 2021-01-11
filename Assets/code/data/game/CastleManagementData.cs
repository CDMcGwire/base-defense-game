using data.refvalues;
using UnityEngine;

namespace data.game {
public class CastleManagementData : MonoBehaviour { 
#pragma warning disable 0649
	[SerializeField] private ReactiveVal<long> health = new ReactiveVal<long>();
	[SerializeField] private ReactiveVal<long> damage = new ReactiveVal<long>();
#pragma warning restore 0649

	public ReactiveVal<long> Health => health;
	public ReactiveVal<long> Damage => damage;
}
}