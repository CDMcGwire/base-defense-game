using combat.castle;
using data.reactive;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ui.town {
public class TurretStoreElement : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private TurretData data;
	[Space(10)]
	[SerializeField] private TMP_Text typeDisplay;
	[SerializeField] private TMP_Text countDisplay;
	[SerializeField] private TMP_Text tierDisplay;
	[SerializeField] private PurchaseButton purchaseButton;
	[SerializeField] private PurchaseButton upgradeButton;
	[SerializeField] private Image imageHolder;
	[SerializeField] private TMP_Text descriptionBox;
#pragma warning restore 0649
	public TurretData Data => data;
	public PurchaseButton PurchaseButton => purchaseButton;
	public PurchaseButton UpgradeButton => upgradeButton;

	private void Start() {
		typeDisplay.text = data.Type;
		imageHolder.sprite = data.Sprite;
		descriptionBox.text = data.Description;
	}

	public void Wire(
		IRxReadonlyVal<int> count,
		IRxReadonlyVal<int> tier
	) {
		count.Bind(value => UpdateNumberDisplay(countDisplay, value));
		tier.Bind(value => UpdateNumberDisplay(tierDisplay, value));
	}

	private static void UpdateNumberDisplay(TMP_Text display, int value)
		=> display.text = $"{value:N0}";
}
}