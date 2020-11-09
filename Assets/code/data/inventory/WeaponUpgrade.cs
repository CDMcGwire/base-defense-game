using System.Collections.Generic;
using combat.effects.core;
using UnityEngine;

namespace data.inventory {
[CreateAssetMenu(fileName = "new-weapon-upgrade", menuName = "Loadout/Weapon Upgrade", order = 0)]
public class WeaponUpgrade : ScriptableObject {
#pragma warning disable 0649
	[SerializeField] private PayloadDataSource payloadData;
	[SerializeField] private List<string> includes;
	[SerializeField] private List<string> excludes;
#pragma warning restore 0649
}
}