using System.Collections.Generic;
using combat.components;
using data.inventory;
using data.refvalues;
using UnityEngine;

namespace data.game {
public class PlayerLoadoutData : MonoBehaviour, ISerializationCallbackReceiver {
#pragma warning disable 0649
	[SerializeField] private ReactiveVal<PlayerCharacter> playerCharacter = new ReactiveVal<PlayerCharacter>();
	[SerializeField] private ReactiveList<InventoryItem> ownedItems = new ReactiveList<InventoryItem>();
	[SerializeField] private ReactiveVal<int> weaponSlots = new ReactiveVal<int>();
	[SerializeField] private ReactiveVal<int> activeWeaponSlot = new ReactiveVal<int>();
	[SerializeField] private ReactiveList<InventoryItem> equippedWeapons = new ReactiveList<InventoryItem>();
	[SerializeField] private ReactiveVal<int> toolSlots = new ReactiveVal<int>();
	[SerializeField] private ReactiveList<InventoryItem> equippedTools = new ReactiveList<InventoryItem>();
#pragma warning restore 0649

	public HashSet<string> UniqueItems { get; } = new HashSet<string>();

	public ReactiveVal<PlayerCharacter> PlayerCharacter => playerCharacter;
	public ReactiveList<InventoryItem> OwnedItems => ownedItems;
	public ReactiveVal<int> WeaponSlots => weaponSlots;
	public ReactiveVal<int> ActiveWeaponSlot => activeWeaponSlot;
	public ReactiveList<InventoryItem> EquippedWeapons => equippedWeapons;
	public ReactiveVal<int> ToolSlots => toolSlots;
	public ReactiveList<InventoryItem> EquippedTools => equippedTools;

	public void OnBeforeSerialize() { }

	public void OnAfterDeserialize() {
		while (equippedWeapons.Count < weaponSlots.Current)
			equippedWeapons.Add(null);
		while (equippedWeapons.Count > weaponSlots.Current)
			equippedWeapons.RemoveLast();
		if (activeWeaponSlot.Current < 0
		    || activeWeaponSlot.Current >= weaponSlots.Current)
			activeWeaponSlot.Current = 0;
		
		while (equippedTools.Count < toolSlots.Current)
			equippedTools.Add(null);
		while (equippedTools.Count > toolSlots.Current)
			equippedTools.RemoveLast();

		var duplicates = new List<int>();
		for (var i = 0; i < ownedItems.Count; i++) {
			var item = ownedItems[i];
			if (!item.Unique) continue;
			if (UniqueItems.Contains(item.name))
				duplicates.Add(i);
			else UniqueItems.Add(item.name);
		}
		foreach (var index in duplicates)
			ownedItems.RemoveAt(index);
	}
}
}