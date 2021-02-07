using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ui.input {
/// <summary>
/// A wrapper over the TMP_Dropdown component which adds some common behavioral
/// logic for equipment interfaces.
/// </summary>
public class Slot : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private TMP_Dropdown dropdown;
	[SerializeField] private GameObject disabledPlaceholder;
	[Space(10)]
	[SerializeField] private string emptyText;
	[SerializeField] private Sprite emptySprite;
	[Space(10)]
	[SerializeField] private UnityEvent<int> onValueChanged = new UnityEvent<int>();
#pragma warning restore 0649

	/// <summary>
	/// The currently selected dropdown value.
	/// </summary>
	public int Value {
		get => dropdown.value;
		set {
			dropdown.SetValueWithoutNotify(value < 0 ? 0 : value + 1);
			dropdown.RefreshShownValue();
		}
	}

	/// <summary>
	/// Instead of just using the Dropdown 'interactable' option
	/// </summary>
	public bool Interactable {
		get => dropdown.gameObject.activeSelf;
		set {
			dropdown.gameObject.SetActive(value);
			disabledPlaceholder.SetActive(!value);
		}
	}

	public UnityEvent<int> OnValueChanged => onValueChanged;

	private void Start()
		=> dropdown.onValueChanged.AddListener(index => onValueChanged.Invoke(index - 1));

	public void Populate(IEnumerable<TMP_Dropdown.OptionData> options) {
		dropdown.options.Clear();
		dropdown.options.Add(new TMP_Dropdown.OptionData(emptyText, emptySprite));
		dropdown.options.AddRange(options);
	}

	public void Insert(int index, TMP_Dropdown.OptionData option) {
		index++;
		if (index < 1)
			return;
		dropdown.options.Insert(index, option);
	}

	public void RemoveAt(int index) {
		index++;
		if (index < 1 || index >= dropdown.options.Count)
			return;
		dropdown.options.RemoveAt(index);
	}

	public void Set(int index, TMP_Dropdown.OptionData option) {
		index++;
		if (index < 1 || index >= dropdown.options.Count)
			return;
		dropdown.options[index] = option;
		// Don't refresh the shown value, as the player may not have selected it.
	}

	public void SetShown(string text, Sprite sprite) {
		if (dropdown.captionText != null)
			dropdown.captionText.text = text;
		if (dropdown.captionImage != null)
			dropdown.captionImage.sprite = sprite;
	}
}
}