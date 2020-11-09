using System;
using System.Collections.Generic;
using combat.weapon;
using data.game;
using data.inventory;
using data.loadout;
using UnityEngine;

namespace ui.town {
public class LoadoutMenu : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private PlayerLoadoutService loadout;
	[SerializeField] private LoadoutSlotGroup weaponSlots;
	[SerializeField] private LoadoutSlotGroup toolSlots;
#pragma warning restore 0649

	private void OnEnable() {
		weaponSlots.PopulateList(
			loadout.OwnedItemsOfType<Weapon>(),
			loadout.EquippedWeapons,
			loadout.EquipWeapon
		);
		toolSlots.PopulateList(
			loadout.OwnedItemsOfType<ToolData>(),
			loadout.EquippedTools,
			loadout.EquipTool
		);
	}
}

[Serializable]
public class LoadoutSlotGroup {
#pragma warning disable 0649
	[SerializeField] private List<LoadoutSlotElement> slots;
#pragma warning restore 0649

	private IReadOnlyList<InventoryItem> slotableItems = new List<InventoryItem>();
	private int[] slotValues;

	public void PopulateList(
		IReadOnlyList<InventoryItem> items,
		IReadOnlyList<InventoryItem> equipped,
		Action<int, InventoryItem> onSlotUpdated
	) {
		slotableItems = items;

		// Enable only as many slots as can be equipped and can be shown.
		var maxSlots = Math.Min(equipped.Count, slots.Count);
		for (var i = 0; i < slots.Count; i++)
			slots[i].SetEnabled(i < maxSlots);

		// Set the initial slot indices.
		slotValues = new int[maxSlots];
		for (var i = 0; i < maxSlots; i++) {
			// Default to empty.
			slotValues[i] = -1;
			// Lookup the index of the slotable item if present and set that as the initial index.
			for (var j = 0; j < slotableItems.Count; j++)
				if (slotableItems[j] == equipped[i])
					slotValues[i] = j;
			// Note: If an unowned item is equipped, it will not be reselectable from this menu if changed.
		}

		// Initialize the dropdown lists.
		for (var i = 0; i < maxSlots; i++)
			slots[i].Populate(items, slotValues[i], equipped[i]);

		// Clear out any other listeners on the slots.
		foreach (var slot in slots)
			slot.OnValueChanged.RemoveAllListeners();

		for (var si = 0; si < slots.Count; si++) {
			var slotIndex = si;

			void ValueChangedAction(int dropdownValue) {
				var itemIndex = dropdownValue - 1;
				// It clearing the slot.
				if (itemIndex < 0) {
					slotValues[slotIndex] = -1;
					onSlotUpdated(slotIndex, null);
					return;
				}
				// Check for slots with the target item already equipped.
				for (var dupeSlot = 0; dupeSlot < slotValues.Length; dupeSlot++) {
					// Skip the triggering slot.
					if (dupeSlot == slotIndex) continue;
					// Skip slots that aren't the item being equipped.
					if (slotValues[dupeSlot] != itemIndex) continue;
					// If a match is found, begin a swap.
					var previousItemIndex = slotValues[slotIndex];
					var swapItem = previousItemIndex > 0 
						? slotableItems[previousItemIndex] 
						: null;
					onSlotUpdated(dupeSlot, swapItem);
					slots[dupeSlot].ManualSelect(previousItemIndex + 1);
					break;
				}
				// Finish update of the target slot.
				slotValues[slotIndex] = itemIndex;
				onSlotUpdated(slotIndex, slotableItems[itemIndex]);
			}

			slots[si].OnValueChanged.AddListener(ValueChangedAction);
		}
	}
}
}