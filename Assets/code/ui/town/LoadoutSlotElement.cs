using System.Collections;
using System.Collections.Generic;
using data.inventory;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ui.town {
public class LoadoutSlotElement : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private TMP_Dropdown dropdown;
	[SerializeField] private Sprite emptySelectionSprite;
	[SerializeField] private GameObject disabledPlaceholder;
#pragma warning restore 0649

	public UnityEvent<int> OnValueChanged => dropdown.onValueChanged;

	public void Populate(IReadOnlyList<InventoryItem> items, int initialIndex, InventoryItem initialItem) {
		dropdown.options.Clear();
		dropdown.options.Add(new TMP_Dropdown.OptionData("Empty", emptySelectionSprite));
		foreach (var item in items)
			dropdown.options.Add(new TMP_Dropdown.OptionData(item.DisplayName, item.Sprite));

		// The initial value may not be a selectable value, but should still be shown until changed.
		dropdown.value = Mathf.Clamp(initialIndex + 1, 0, items.Count);
		if (initialItem != null)
			StartCoroutine(SetInitialDisplay(initialItem.name, initialItem.Sprite));
	}

	public void SetEnabled(bool value) {
		dropdown.gameObject.SetActive(value);
		disabledPlaceholder.SetActive(!value);
	}

	public void ManualSelect(int value)
		=> dropdown.value = value;

	private IEnumerator SetInitialDisplay(string itemName, Sprite itemSprite) {
		yield return new WaitForEndOfFrame();
		if (dropdown.captionText != null)
			dropdown.captionText.text = itemName;
		if (dropdown.captionImage != null)
			dropdown.captionImage.sprite = itemSprite;
	}
}
}