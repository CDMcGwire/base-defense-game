using System.Collections.Generic;
using data.inventory;
using UnityEngine;

namespace data.store.filter {
public abstract class StoreFilter : ScriptableObject {
	/// <summary>
	/// Filter out any items in the input list that may not be sold to the player.
	/// </summary>
	/// <param name="inputList">The initial list of items.</param>
	/// <param name="outputList">The list to store the filtered list of items.</param>
	public void Process(IReadOnlyList<InventoryItem> inputList, List<InventoryItem> outputList) {
		outputList.Clear();
		BeforeProcess();
		foreach (var entry in inputList)
			if (!ShouldFilter(entry))
				outputList.Add(entry);
	}

	/// <summary>
	/// Called before processing a list to allow for batch optimizations to be
	/// setup.
	/// </summary>
	protected virtual void BeforeProcess() { }

	/// <summary>
	/// Determine if an item should not pass through this filter. This is where
	/// the batch based logic should be implemented. BeforeProcess() will have
	/// already been called before this for the sake of optimization strategies.
	/// </summary>
	/// <param name="item">The item to check.</param>
	/// <returns>True if the item should not pass through.</returns>
	protected abstract bool ShouldFilter(InventoryItem item);

	/// <summary>
	/// Determine if an item should not pass through this filter. This is the
	/// version that should implement logic for checking only one entry.
	/// BeforeProcess() is not guaranteed to have been called, thus no batch
	/// optimizations can be made.
	/// </summary>
	/// <param name="item">The item to check.</param>
	/// <returns>True if the item should not pass through.</returns>
	public abstract bool ShouldFilterSingle(InventoryItem item);

	// DESIGN NOTE: If filters need access to service data, they should create a serialized field for that data.
}
}