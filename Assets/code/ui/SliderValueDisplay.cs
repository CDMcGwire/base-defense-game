using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueDisplay : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI display;

	[SerializeField] private Slider slider;

	private void UpdateDisplay(float value) => display.text = $"{value:0.##}";

	private void Awake() => UpdateDisplay(slider.value);

	private void OnEnable() => slider.onValueChanged.AddListener(UpdateDisplay);

	private void OnDisable() => slider.onValueChanged.RemoveListener(UpdateDisplay);
}