using System;
using System.Collections.Generic;
using data.reactive;
using utility.extensions;

namespace data.inventory {
public readonly struct InventorySlotManager<T> where T : class, IEquatable<T> {
	private readonly Dictionary<T, int> availableItems;
	private readonly IList<T> managedSlots;

	public InventorySlotManager(
		IRxReadonlyList<T> sourceItems,
		IList<T> managedSlots // Length should be managed externally
	) {
		availableItems = new Dictionary<T, int>();
		this.managedSlots = managedSlots;
		PopulateAvailableItems(sourceItems);

		sourceItems.OnRestructure += SourceRestructureHandler;
		sourceItems.OnItemAdded += SourceItemAddedHandler;
		sourceItems.OnItemRemoved += SourceItemRemovedHandler;
	}

	private void PopulateAvailableItems(IEnumerable<T> sourceItems) {
		foreach (var item in sourceItems) {
			if (availableItems.ContainsKey(item))
				availableItems[item]++;
			else availableItems[item] = 1;
		}

		for (var i = 0; i < managedSlots.Count; i++) {
			var item = managedSlots[i];
			if (item == null) continue;
			if (availableItems.ContainsKey(item)
			    && availableItems[item] > 0)
				availableItems[item]--;
			else managedSlots[i] = null;
		}
	}

	/// <summary>
	/// Tries to place the provided item into the designated slot. Will check if
	/// there are enough copies of the item to fill the slot, and if not, will
	/// swap the contents of the target slot with the first slot found to contain
	/// a copy of the provided item.
	///
	/// If the provided item is null, then the slot will be emptied and whatever
	/// item was previously in the slot will be freed up.
	///
	/// If the provided item is not in the pool of available items, then no
	/// change will be made.
	/// </summary>
	/// <param name="slot">The index of the slot to modify.</param>
	/// <param name="value">The item to insert into the slot.</param>
	public void SetSlotValue(int slot, T value) {
		if (managedSlots.IndexOutOfBounds(slot))
			return;
		if (value == null) {
			EmptySlot(slot);
			return;
		}
		if (!availableItems.ContainsKey(value))
			return;
		if (managedSlots[slot] == value)
			return;
		if (availableItems[value] < 1)
			FindAndSwap(slot, value);
		else FillSlot(slot, value);
	}

	/// <summary>
	/// Called if a slot changes to the "empty" option.
	/// </summary>
	/// <param name="slotIndex">The index of the changing slot.</param>
	private void EmptySlot(int slotIndex) {
		if (managedSlots.IndexOutOfBounds(slotIndex))
			return;
		var lastItem = managedSlots[slotIndex];
		managedSlots[slotIndex] = null;
		if (lastItem != null)
			availableItems[lastItem]++;
	}

	/// <summary>
	/// Called if the target item has already been placed in the max number of
	/// slots.
	/// </summary>
	/// <param name="slot">The index of the slot that triggered the swap.</param>
	/// <param name="value">The item to swap.</param>
	private void FindAndSwap(int slot, T value) {
		var swapSlot = -1;
		for (var i = 0; i < managedSlots.Count; i++) {
			if (managedSlots[i] != value) continue;
			swapSlot = i;
			break;
		}
		managedSlots[swapSlot] = managedSlots[slot];
		managedSlots[slot] = value;
	}

	/// <summary>
	/// Fill a slot with an available item.
	/// </summary>
	/// <param name="slot">The index of the slot that triggered the swap.</param>
	/// <param name="value">The item to swap.</param>
	private void FillSlot(int slot, T value) {
		managedSlots[slot] = value;
		availableItems[value]--;
	}

	private void SourceRestructureHandler(IReadOnlyList<T> sourceItems)
		=> PopulateAvailableItems(sourceItems);

	private void SourceItemAddedHandler(T value, int index) {
		if (availableItems.ContainsKey(value))
			availableItems[value]++;
		else availableItems[value] = 1;
	}

	private void SourceItemRemovedHandler(T value, int index) {
		availableItems[value]--;
		if (availableItems[value] >= 1) return;
		var inUse = false;
		foreach (var item in managedSlots) {
			if (item != value) continue;
			inUse = true;
			break;
		}
		if (!inUse) availableItems.Remove(value);
	}
}
}