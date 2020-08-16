using managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ui.settings {
public class AudioSettingsPanel : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private AudioSettingsManager audioSettingsManager;
	[SerializeField] private Slider masterVolumeSlider;
	[SerializeField] private Toggle muteMasterToggle;
#pragma warning restore 0649

	private UnityAction<float> masterVolumeHandler;
	private UnityAction<bool> muteMasterHandler;

	private void Awake() {
		Debug.Assert(
			audioSettingsManager != null,
			"Sound settings panel is missing a reference to a sound settings manager."
		);
		Debug.Assert(
			masterVolumeSlider != null,
			"Sound settings panel is missing a reference to a master volume slider."
		);
		Debug.Assert(
			muteMasterToggle != null,
			"Sound settings panel is missing a reference to a mute master toggle."
		);
		InitializeAudioSlider(masterVolumeSlider, audioSettingsManager.GameVolume);
		masterVolumeHandler = value => audioSettingsManager.GameVolume = value;

		muteMasterToggle.isOn = audioSettingsManager.MutedMaster;
		muteMasterHandler = value => audioSettingsManager.MutedMaster = value;
	}

	private void OnEnable() {
		masterVolumeSlider.onValueChanged.AddListener(masterVolumeHandler);
		muteMasterToggle.onValueChanged.AddListener(muteMasterHandler);
	}

	private void OnDisable() {
		masterVolumeSlider.onValueChanged.RemoveListener(masterVolumeHandler);
		muteMasterToggle.onValueChanged.RemoveListener(muteMasterHandler);
	}

	private static void InitializeAudioSlider(Slider slider, float value) {
		slider.minValue = 0;
		slider.maxValue = 1;
		slider.value = Mathf.Clamp01(value);
	}
}
}
