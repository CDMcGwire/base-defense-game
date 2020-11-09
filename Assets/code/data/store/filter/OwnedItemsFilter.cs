using System.Collections.Generic;
using System.Linq;
using data.game;
using data.inventory;
using UnityEngine;

namespace data.store.filter {
[CreateAssetMenu(fileName = "owned-items-filter", menuName = "Town/Filter/Owned Items", order = 0)]
public class OwnedItemsFilter : StoreFilter {
#pragma warning disable 0649
	[SerializeField] private PlayerLoadoutService loadout;
#pragma warning restore 0649

	private readonly HashSet<string> ownedItems = new HashSet<string>();
	
	protected override void BeforeProcess() {
		ownedItems.Clear();
		foreach (var item in loadout.OwnedItems)
			ownedItems.Add(item.name);
	}

	protected override bool ShouldFilter(InventoryItem item)
		=> ownedItems.Contains(item.name);

	public override bool ShouldFilterSingle(InventoryItem item)
		=> loadout.OwnedItems.Contains(item);
}
}