using data.reactive;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ui.town {
public class PurchaseButton : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private Button button;
	[SerializeField] private TMP_Text costDisplay;
	[SerializeField] private string costTemplate = "${}";
#pragma warning restore 0649

	private readonly RxExp<long, long, bool> canAfford
		= new RxExp<long, long, bool>((cost, funds) => cost <= funds);
	private readonly RxExp<bool, bool, bool> canPurchase
		= new RxExp<bool, bool, bool>((affordable, available) => affordable && available);

	private void Start()
		=> canPurchase.OnChanged += purchasable => button.interactable = purchasable;

	/// <summary>
	/// Wiring performs all of the permanent expression attachments needed by the
	/// UI element. Anything given in this method is assumed to have a similar
	/// lifetime as the UI element, thus unsubscribing will not be necessary as
	/// their initialization and garbage collection are synchronized.
	/// </summary>
	/// <param name="cost">A value or expression that resolves to the cost of the associated item.</param>
	/// <param name="available">A value or expression that indicates whether or not the item is available.</param>
	/// <param name="funds">A value or expression that resolves to the shopping player's current money.</param>
	public void Wire(IRxReadonlyVal<long> cost, IRxReadonlyVal<bool> available, IRxReadonlyVal<long> funds) {
		canAfford.Attach(cost, funds);
		canPurchase.Attach(canAfford, available);
		cost.Bind(UpdateCostDisplay);
		canPurchase.Bind(purchasable => button.interactable = purchasable);
	}

	[NotNull]
	public UnityAction OnPurchase {
		set {
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(value);
		}
	}

	private void UpdateCostDisplay(long cost)
		=> costDisplay.text = costTemplate.Replace("{}", $"{cost:N0}");
}
}