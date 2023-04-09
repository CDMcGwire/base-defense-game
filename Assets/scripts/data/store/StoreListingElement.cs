using System;
using data.inventory;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace data.store {
public class StoreListingElement : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private InventoryItem item;
	[SerializeField] private TMP_Text nameDisplay;
	[SerializeField] private TMP_Text priceDisplay;
	[SerializeField] private UnityEvent onPurchase;
	[SerializeField] private UnityEvent onCanAfford;
	[SerializeField] private UnityEvent onCannotAfford;
#pragma warning restore 0649

	private bool purchasable = true;

	public InventoryItem Item {
		get => item;
		set {
			item = value;
			if (value == null) return;
			nameDisplay.text = value.DisplayName;
			priceDisplay.text = $"{value.Price:N0}";
		}
	}

	public UnityEvent OnPurchase => onPurchase;
	public UnityEvent OnCanAfford => onCanAfford;
	public UnityEvent OnCannotAfford => onCannotAfford;
	
	/// <summary>
	/// The purchase action is the specific action to perform for the store
	/// system. Unlike OnPurchase, this will only hold one delegate and will be
	/// populated by the stocking SalesAssociate via code.
	/// </summary>
	public Action PurchaseAction { get; set; }

	public bool Purchasable {
		set {
			if (purchasable == value) return;
			purchasable = value;
			if (purchasable) OnCanAfford.Invoke();
			else OnCannotAfford.Invoke();
		}
	}

	[UsedImplicitly]
	public void Purchase() {
		PurchaseAction.Invoke();
		OnPurchase.Invoke();
	}
}
}