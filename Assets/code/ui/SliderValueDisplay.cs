using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ui {
public class SliderValueDisplay : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private TextMeshProUGUI display;
	[SerializeField] private Slider slider;
#pragma warning restore 0649

	private void UpdateDisplay(float value) => display.text = $"{value:0.##}";

	private void Awake() => UpdateDisplay(slider.value);

	private void OnEnable() => slider.onValueChanged.AddListener(UpdateDisplay);

	private void OnDisable() => slider.onValueChanged.RemoveListener(UpdateDisplay);
}
}