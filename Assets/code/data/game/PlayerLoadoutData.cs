using System.Collections.Generic;
using combat.components;
using data.inventory;
using data.reactive;
using UnityEngine;

namespace data.game {
public class PlayerLoadoutData : MonoBehaviour, ISerializationCallbackReceiver {
#pragma warning disable 0649
	[SerializeField] private RxVal<PlayerCharacter> playerCharacter = new RxVal<PlayerCharacter>();
	[SerializeField] private RxList<InventoryItem> ownedItems = new RxList<InventoryItem>();
	[SerializeField] private RxVal<int> weaponSlots = new RxVal<int>();
	[SerializeField] private RxVal<int> activeWeaponSlot = new RxVal<int>();
	[SerializeField] private RxList<InventoryItem> equippedWeapons = new RxList<InventoryItem>();
	[SerializeField] private RxVal<int> toolSlots = new RxVal<int>();
	[SerializeField] private RxList<InventoryItem> equippedTools = new RxList<InventoryItem>();
#pragma warning restore 0649

	public HashSet<string> UniqueItems { get; } = new HashSet<string>();

	public RxVal<PlayerCharacter> PlayerCharacter => playerCharacter;
	public RxList<InventoryItem> OwnedItems => ownedItems;
	public RxVal<int> WeaponSlots => weaponSlots;
	public RxVal<int> ActiveWeaponSlot => activeWeaponSlot;
	public RxList<InventoryItem> EquippedWeapons => equippedWeapons;
	public RxVal<int> ToolSlots => toolSlots;
	public RxList<InventoryItem> EquippedTools => equippedTools;

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