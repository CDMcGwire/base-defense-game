using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ui {
[RequireComponent(typeof(Toggle))]
public class ToggleOnEvent : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private UnityEvent onToggleOn;
#pragma warning restore 0649

	private void Awake() {
		var toggle = GetComponent<Toggle>();
		toggle.onValueChanged.AddListener(isOn => {
			if (isOn) onToggleOn.Invoke();
		});
	}
}
}