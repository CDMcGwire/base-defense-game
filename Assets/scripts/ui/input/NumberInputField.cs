using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ui.input {
[RequireComponent(typeof(TMP_InputField))]
public class NumberInputField : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private UnityEvent<float> onValueChanged;
	[SerializeField] private UnityEvent<string> onInvalidEntry;
#pragma warning restore 0649

	private TMP_InputField inputField;

	[UsedImplicitly] public UnityEvent<float> OnValueChanged => onValueChanged;
	[UsedImplicitly] public UnityEvent<string> OnInvalidEntry => onInvalidEntry;

	private void Awake() {
		inputField = GetComponent<TMP_InputField>();
		inputField.onValueChanged.AddListener(value => {
				if (float.TryParse(value, out var number))
					onValueChanged.Invoke(number);
				else
					onInvalidEntry.Invoke(value);
			}
		);
	}
}
}
