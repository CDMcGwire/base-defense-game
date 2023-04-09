using System;
using System.Collections.Generic;
using combat.castle;
using data.inventory;
using data.reactive;
using UnityEngine;

namespace data.game {
public class CastleManagementData : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private RxVal<long> health = new();
	[SerializeField] private RxVal<long> damage = new();
	[SerializeField] private RxVal<int> moatTier = new();
	[SerializeField] private RxVal<int> turretSlots = new();
	[SerializeField] private RxList<OwnedTurret> equippedTurrets = new();
	[SerializeField] private RxList<OwnedTurret> ownedTurrets = new();
#pragma warning restore 0649

	public Dictionary<string, OwnedTurret> turretsByType = new();
	public InventorySlotManager<OwnedTurret> EquippedTurretManager;

	public RxVal<long> Health => health;
	public RxVal<long> Damage => damage;
	public RxVal<int> MoatTier => moatTier;
	public RxVal<int> TurretSlots => turretSlots;
	public RxList<OwnedTurret> EquippedTurrets => equippedTurrets;
	public RxList<OwnedTurret> OwnedTurrets => ownedTurrets;
}

[Serializable]
public class OwnedTurret : IEquatable<OwnedTurret> {
#pragma warning disable 0649
	[SerializeField] private TurretData data;
	[SerializeField] private RxVal<int> count;
	[SerializeField] private RxVal<int> tier;
#pragma warning restore 0649

	public TurretData Data => data;

	public RxVal<int> Count => count;

	public RxVal<int> Tier => tier;

	public OwnedTurret(TurretData data, int numberOwned, int tier) {
		this.data = data;
		this.count = new RxVal<int>(numberOwned);
		this.tier = new RxVal<int>(tier);
	}

	public bool Equals(OwnedTurret other) {
		if (ReferenceEquals(null, other)) return false;
		return ReferenceEquals(this, other) || Equals(data, other.data);
	}

	public override bool Equals(object obj) {
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		return obj.GetType() == GetType() && Equals((OwnedTurret) obj);
	}

	public override int GetHashCode()
		=> data != null ? data.GetHashCode() : 0;

	public static bool operator ==(OwnedTurret left, OwnedTurret right)
		=> Equals(left, right);

	public static bool operator !=(OwnedTurret left, OwnedTurret right)
		=> !Equals(left, right);
}
}