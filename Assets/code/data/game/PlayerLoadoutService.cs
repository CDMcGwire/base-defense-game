using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using combat.components;
using combat.weapon;
using data.inventory;
using data.loadout;
using data.reactive;
using data.service;
using UnityEngine;

namespace data.game {
[CreateAssetMenu(fileName = "player-loadout-service", menuName = "Service/Player Loadout", order = 0)]
public class PlayerLoadoutService : DataService<PlayerLoadoutData>, IPersistableService {
#pragma warning disable 0649
	[SerializeField] private PlayerCharacter initialPlayerCharacter;
	[SerializeField] private List<InventoryItem> initialOwnedItems = new List<InventoryItem>();
	[SerializeField] private int initialWeaponSlots = 3;
	[SerializeField] private int initialActiveWeaponSlot = 0;
	[SerializeField] private List<InventoryItem> initialEquippedWeapons = new List<InventoryItem>();
	[SerializeField] private int initialToolSlots = 2;
	[SerializeField] private List<InventoryItem> initialEquippedTools = new List<InventoryItem>();
#pragma warning restore 0649

	public IRxReadonlyVal<PlayerCharacter> PlayerCharacter => Data.PlayerCharacter;
	public IRxReadonlyList<InventoryItem> OwnedItems => Data.OwnedItems;
	public IRxReadonlyVal<int> WeaponSlots => Data.WeaponSlots;
	public IRxReadonlyVal<int> ActiveWeaponSlot => Data.ActiveWeaponSlot;
	public IRxReadonlyList<InventoryItem> EquippedWeapons => Data.EquippedWeapons;
	public IRxReadonlyVal<int> ToolSlots => Data.ToolSlots;
	public IRxReadonlyList<InventoryItem> EquippedTools => Data.EquippedTools;

	public override void Initialize() {
		ChangePlayerCharacter(initialPlayerCharacter);

		// Scrub initial items for duplicates
		Data.UniqueItems.Clear();
		Data.OwnedItems.Clear();
		foreach (var item in initialOwnedItems)
			AddItem(item);

		Data.EquippedWeapons.Clear();
		ChangeWeaponSlotTotal(initialWeaponSlots);
		ChangeActiveWeaponSlot(initialActiveWeaponSlot);
		for (var i = 0; i < initialWeaponSlots; i++) {
			var item = i < initialEquippedWeapons.Count
				? initialEquippedWeapons[i]
				: null;
			EquipWeapon(i, item);
		}

		Data.EquippedTools.Clear();
		ChangeToolSlotTotal(initialToolSlots);
		for (var i = 0; i < initialToolSlots; i++) {
			var item = i < initialEquippedTools.Count
				? initialEquippedTools[i]
				: null;
			EquipTool(i, item);
		}
	}

	public void ChangePlayerCharacter(PlayerCharacter character)
		=> Data.PlayerCharacter.Current = character;

	public void ChangeWeaponSlotTotal(int value) {
		if (value < 1) return;
		Data.WeaponSlots.Current = value;
		Data.EquippedWeapons.SizeTo(value);
	}

	public void ChangeActiveWeaponSlot(int value) {
		if (value < 0 || value >= Data.WeaponSlots.Current)
			return;
		Data.ActiveWeaponSlot.Current = value;
	}

	public void ChangeToolSlotTotal(int value) {
		if (value < 1) return;
		Data.ToolSlots.Current = value;
		Data.EquippedTools.SizeTo(value);
	}

	public void AddItem(InventoryItem item) {
		if (item == null) return;
		if (item.Unique) {
			if (Data.UniqueItems.Contains(item.name)) return;
			Data.UniqueItems.Add(item.name);
		}
		Data.OwnedItems.Add(item);
	}

	public void EquipWeapon(int slot, InventoryItem item) {
		if (slot < 0 || slot >= Data.WeaponSlots.Current) return;
		if (item == null || !item.HasDataType<Weapon>())
			Data.EquippedWeapons[slot] = null;
		else Data.EquippedWeapons[slot] = item;
	}

	public void EquipTool(int slot, InventoryItem item) {
		if (slot < 0 || slot >= Data.ToolSlots.Current) return;
		if (item == null || !item.HasDataType<ToolData>())
			Data.EquippedTools[slot] = null;
		else Data.EquippedTools[slot] = item;
	}

	public IReadOnlyList<InventoryItem> OwnedItemsOfType<T>() {
		var filteredList = new List<InventoryItem>();
		foreach (var item in Data.OwnedItems)
			if (item.HasDataType<T>())
				filteredList.Add(item);
		return filteredList;
	}

	public async Task WriteLines(StreamWriter stream)
		=> await stream.WriteLineAsync(JsonUtility.ToJson(Data));

	public async Task ReadLines(SectionReader stream)
		=> JsonUtility.FromJsonOverwrite(await stream.ReadLineAsync(), Data);
}
}