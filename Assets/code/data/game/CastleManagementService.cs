using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using combat.castle;
using combat.components;
using data.inventory;
using data.reactive;
using data.service;
using JetBrains.Annotations;
using UnityEngine;
using utility.extensions;

namespace data.game {
[CreateAssetMenu(fileName = "castle-management-service", menuName = "Service/Castle Management", order = 0)]
public class CastleManagementService : DataService<CastleManagementData>, IPersistableService {
#pragma warning disable 0649
	[SerializeField] private long initialHealth = 100;
	[SerializeField] private long initialDamage = 0;
	[SerializeField] private int initialMoatTier = 0;
	[SerializeField] private int initialTurretSlots = 3;
	[SerializeField] private List<OwnedTurret> initialOwnedTurrets;
	[SerializeField] private List<OwnedTurret> initialEquippedTurrets;
#pragma warning restore 0649

	public long BaseHealth => initialHealth;

	public IRxReadonlyVal<long> Health => Data.Health;
	public IRxReadonlyVal<long> Damage => Data.Damage;
	public IRxReadonlyVal<int> MoatTier => Data.MoatTier;
	public IRxReadonlyVal<int> TurretSlots => Data.TurretSlots;
	public IRxReadonlyList<OwnedTurret> OwnedTurrets => Data.OwnedTurrets;
	public IRxReadonlyList<OwnedTurret> EquippedTurrets => Data.EquippedTurrets;

	private void OnValidate() {
		while (initialEquippedTurrets.Count < initialTurretSlots)
			initialEquippedTurrets.Add(null);
		if (initialEquippedTurrets.Count <= initialTurretSlots)
			return;
		var truncatedList = new List<OwnedTurret>();
		for (var i = 0; i < initialTurretSlots; i++)
			truncatedList.Add(initialEquippedTurrets[i]);
		initialEquippedTurrets = truncatedList;
	}

	public override void Initialize() {
		Data.Health.Current = initialHealth;
		Data.Damage.Current = initialDamage;
		Data.MoatTier.Current = initialMoatTier;
		Data.TurretSlots.Current = initialTurretSlots;
		Data.OwnedTurrets.Repopulate(initialOwnedTurrets);
		Data.EquippedTurrets.Repopulate(initialEquippedTurrets);
		Data.turretsByType.Clear();
		foreach (var turret in Data.OwnedTurrets)
			Data.turretsByType[turret.Data.Type] = turret;
		Data.EquippedTurretManager = new InventorySlotManager<OwnedTurret>(
			Data.OwnedTurrets,
			Data.EquippedTurrets
		);
	}

	public void SetHealth(long value) {
		if (value < 1) value = 1;
		Data.Health.Current = value;
		// It's expected that systems responsible for declaring the base as
		// destroyed will only be subscribed to the Damage value, thus it updates
		// after.
		if (Data.Damage.Current >= value)
			Data.Damage.Current = value - 1;
	}

	public void ChangeMoatTier(int tier)
		=> Data.MoatTier.Current = tier < 0 ? 0 : tier;

	public void ChangeTurretSlots(int slots) {
		Data.TurretSlots.Current = slots < 0 ? 0 : slots;
		Data.EquippedTurrets.SizeTo(slots);
	}

	public void PurchaseTurret(TurretData turretData) {
		if (Data.turretsByType.ContainsKey(turretData.Type))
			Data.turretsByType[turretData.Type].Count.Current++;
		else {
			var turret = new OwnedTurret(turretData, 1, 0);
			Data.OwnedTurrets.Add(turret);
			Data.turretsByType[turretData.Type] = turret;
		}
	}

	public void UpgradeTurret(string turretType) {
		if (Data.turretsByType.ContainsKey(turretType))
			Data.turretsByType[turretType].Tier.Current++;
	}

	public void RecordDamage(long value)
		=> Data.Damage.Current += value;

	[UsedImplicitly]
	public void RecordDamage(DamageReport report)
		=> RecordDamage(report.damage);

	public OwnedTurret OwnedTurretByType(string type)
		=> Data.turretsByType.ContainsKey(type) ? Data.turretsByType[type] : null;

	public async Task WriteLines(StreamWriter stream)
		=> await stream.WriteLineAsync(JsonUtility.ToJson(Data));

	public async Task ReadLines(SectionReader stream)
		=> JsonUtility.FromJsonOverwrite(await stream.ReadLineAsync(), Data);

	public void EquipTurret(int index, string type) {
		if (Data.EquippedTurrets.IndexOutOfBounds(index))
			return;
		if (type == null) {
			Data.EquippedTurretManager.SetSlotValue(index, null);
		}
		else {
			var turret = OwnedTurretByType(type);
			Data.EquippedTurretManager.SetSlotValue(index, turret);
		}
	}
}
}