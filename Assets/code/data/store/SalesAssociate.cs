using System.Collections.Generic;
using data.game;
using data.inventory;
using data.refvalues;
using data.store.filter;
using JetBrains.Annotations;
using UnityEngine;

namespace data.store {
public class SalesAssociate : RefValueObserver<long> {
#pragma warning disable 0649
	[SerializeField] private StoreInventory storeInventory;
	[SerializeField] private StoreShelf storeShelf;
	[SerializeField] private RefValue<long> playerWallet;
	[SerializeField] private PlayerLoadoutService playerLoadout;
	[SerializeField] private List<StoreFilter> filters;
#pragma warning restore 0649

	protected override RefValue<long> Reference => playerWallet;

	[UsedImplicitly]
	public void StockShelves() {
		var items = BuildPresentableList();
		var elements = storeShelf.ClearShelfSpace(items.Count);
		for (var i = 0; i < items.Count; i++) {
			var element = elements[i];
			var item = items[i];
			element.Item = item;
			element.PurchaseAction = () => HandlePurchase(element);
		}
		UpdatePurchasable();
	}

	private IReadOnlyList<InventoryItem> BuildPresentableList() {
		var inputList = new List<InventoryItem>(storeInventory.Items);
		var outputList = new List<InventoryItem>();
		foreach (var filter in filters) {
			filter.Process(inputList, outputList);
			var placeholder = inputList;
			inputList = outputList;
			outputList = placeholder;
		}
		// Because of the swapping, the input list will have the final value.
		return inputList;
	}

	private void HandlePurchase(StoreListingElement listing) {
		if (playerWallet.Value.Current < listing.Item.Price)
			return;
		playerWallet.Value.Current -= listing.Item.Price;
		playerLoadout.AddItem(listing.Item);
		foreach (var filter in filters) {
			if (!filter.ShouldFilterSingle(listing.Item)) 
				continue;
			storeShelf.Remove(listing);
			break;
		}
	}

	protected override void OnValueChanged(long previous, long current)
		=> UpdatePurchasable();

	private void UpdatePurchasable() {
		foreach (var listing in storeShelf.ListedItems)
			listing.Purchasable = listing.Item.Price <= playerWallet.Value.Current;
	}
}
}