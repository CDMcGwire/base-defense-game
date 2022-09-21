using System.Collections.Generic;
using UnityEngine;

namespace data.store {
public class StoreShelf : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private Transform listParent;
	[SerializeField] private StoreListingElement listElement;
#pragma warning restore 0649

	private readonly List<StoreListingElement> elementPool = new();
	private readonly List<StoreListingElement> activeElements = new();

	public IReadOnlyList<StoreListingElement> ListedItems => activeElements;

	public IReadOnlyList<StoreListingElement> ClearShelfSpace(int count) {
		while (elementPool.Count < count)
			elementPool.Add(Instantiate(listElement, listParent));

		activeElements.Clear();
		var i = 0;
		for (; i < count; i++) {
			elementPool[i].gameObject.SetActive(true);
			activeElements.Add(elementPool[i]);
		}
		for (; i < elementPool.Count; i++)
			elementPool[i].gameObject.SetActive(false);
		return activeElements;
	}

	public void Remove(StoreListingElement listing) {
		activeElements.Remove(listing);
		listing.gameObject.SetActive(false);
	}
}
}