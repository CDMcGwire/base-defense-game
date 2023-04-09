using System.Collections.Generic;
using data.inventory;
using UnityEngine;

namespace data.store {
[CreateAssetMenu(fileName = "store-inventory", menuName = "Town/Store Inventory", order = 0)]
public class StoreInventory : ScriptableObject {
#pragma warning disable 0649
	[SerializeField] private List<InventoryItem> items;
#pragma warning restore 0649

	public IReadOnlyList<InventoryItem> Items => items;
}
}