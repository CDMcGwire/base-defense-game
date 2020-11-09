using System;
using System.Collections.Generic;
using combat.components;
using combat.weapon;
using data.inventory;
using data.loadout;
using data.service;
using UnityEngine;
using utility.extensions;

namespace data.game {
[CreateAssetMenu(fileName = "player-loadout-service", menuName = "Service/Player Loadout", order = 0)]
public class PlayerLoadoutService : Service<PlayerLoadout> {
#pragma warning disable 0649
	[SerializeField] private PlayerLoadoutData initialLoadout;
#pragma warning restore 0649

	public PlayerCharacter PlayerCharacter {
		get => DataComponent == null 
			? null 
			: DataComponent.LoadoutData.PlayerCharacter;
		set {
			if (DataComponent != null) 
				DataComponent.LoadoutData.PlayerCharacter = value;
		}
	}

	public int ActiveWeaponSlot {
		get => DataComponent == null ? 0 : DataComponent.LoadoutData.ActiveWeaponSlot;
		set {
			if (DataComponent != null) DataComponent.LoadoutData.ActiveWeaponSlot = value;
		}
	}
	
	public IReadOnlyList<InventoryItem> OwnedItems
		=> DataComponent == null
			? Array.Empty<InventoryItem>()
			: DataComponent.LoadoutData.OwnedItems;

	public IReadOnlyList<InventoryItem> EquippedWeapons
		=> DataComponent == null
			? Array.Empty<InventoryItem>()
			: DataComponent.LoadoutData.EquippedWeapons;

	public IReadOnlyList<InventoryItem> EquippedTools
		=> DataComponent == null
			? Array.Empty<InventoryItem>()
			: DataComponent.LoadoutData.EquippedTools;

	protected override void InitializeDataObject(PlayerLoadout data)
		=> data.LoadoutData = new PlayerLoadoutData(initialLoadout);

	public void Clear()
		=> DataComponent.LoadoutData = new PlayerLoadoutData(initialLoadout);

	public PlayerLoadoutData BuildSaveData()
		=> DataComponent.LoadoutData;

	public void LoadFromData(PlayerLoadoutData saveData)
		=> DataComponent.LoadoutData = saveData;

	public void Add(InventoryItem item)
		=> DataComponent.LoadoutData.Add(item);

	public void EquipWeapon(int slot, InventoryItem item)
		=> DataComponent.LoadoutData.EquipWeapon(slot, item);

	public void EquipTool(int slot, InventoryItem item)
		=> DataComponent.LoadoutData.EquipTool(slot, item);

	public IReadOnlyList<InventoryItem> OwnedItemsOfType<T>() {
		var filteredList = new List<InventoryItem>();
		foreach (var item in OwnedItems)
			if (item.HasDataType<T>())
				filteredList.Add(item);
		return filteredList;
	}
}

[Serializable]
public class PlayerLoadoutData : ISerializationCallbackReceiver {
#pragma warning disable 0649
	[SerializeField] private PlayerCharacter playerCharacter;
	[SerializeField] private List<InventoryItem> ownedItems;
	[SerializeField] private int weaponSlots = 3;
	[SerializeField] private int activeWeaponSlot;
	[SerializeField] private InventoryItem[] equippedWeapons;
	[SerializeField] private int toolSlots = 2;
	[SerializeField] private InventoryItem[] equippedTools;
#pragma warning restore 0649

	private readonly HashSet<string> uniqueItems = new HashSet<string>();

	public PlayerCharacter PlayerCharacter {
		get => playerCharacter;
		set => playerCharacter = value;
	}

	public int ActiveWeaponSlot {
		get => activeWeaponSlot;
		set => activeWeaponSlot = Mathf.Clamp(value, 0, weaponSlots - 1);
	}

	public IReadOnlyList<InventoryItem> OwnedItems => ownedItems;
	public IReadOnlyList<InventoryItem> EquippedWeapons => equippedWeapons;
	public IReadOnlyList<InventoryItem> EquippedTools => equippedTools;

	public PlayerLoadoutData() {
		ownedItems = new List<InventoryItem>();
		equippedWeapons = new InventoryItem[weaponSlots];
		equippedTools = new InventoryItem[toolSlots];
	}

	public PlayerLoadoutData(PlayerLoadoutData other) {
		playerCharacter = other.playerCharacter;
		ownedItems = new List<InventoryItem>(other.ownedItems);

		weaponSlots = other.weaponSlots;
		activeWeaponSlot = other.activeWeaponSlot;
		equippedWeapons = new InventoryItem[weaponSlots];
		Array.Copy(other.equippedWeapons, equippedWeapons, weaponSlots);

		toolSlots = other.toolSlots;
		equippedTools = new InventoryItem[toolSlots];
		Array.Copy(other.equippedTools, equippedTools, toolSlots);
	}

	public void Add(InventoryItem item) {
		if (item == null) return;
		if (item.Unique) {
			if (uniqueItems.Contains(item.name)) return;
			uniqueItems.Add(item.name);
		}
		ownedItems.Add(item);
	}

	public void EquipWeapon(int slot, InventoryItem item) {
		if (slot < 0 || slot >= weaponSlots) return;
		if (item == null || !item.HasDataType<Weapon>()) {
			equippedWeapons[slot] = null;
			return;
		}
		equippedWeapons[slot] = item;
	}

	public void EquipTool(int slot, InventoryItem item) {
		if (slot < 0 || slot >= toolSlots) return;
		if (item == null || !item.HasDataType<ToolData>()) {
			equippedTools[slot] = null;
			return;
		}
		equippedTools[slot] = item;
	}

	public void OnBeforeSerialize() { }

	public void OnAfterDeserialize() {
		if (equippedWeapons.Length != weaponSlots)
			equippedWeapons = equippedWeapons.CopyToSize(weaponSlots);
		if (equippedTools.Length != toolSlots)
			equippedTools = equippedTools.CopyToSize(toolSlots);
	}
}
}