using System;
using System.Collections.Generic;
using managers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ui.settings {
public class DisplaySettingsPanel : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private DisplaySettingsManager displaySettingsManager;
	[Space(12), SerializeField] private TMP_Dropdown resolutionDropdown;
	[SerializeField] private TMP_Dropdown fullscreenModeDropdown;
	[SerializeField] private Toggle vsyncToggle;
	[SerializeField] private Slider targetFramerateSlider;
#pragma warning restore 0649

	private UnityAction<int> resolutionChangeHandler;
	private UnityAction<int> fsModeChangeHandler;
	private UnityAction<bool> vsyncChangeHandler;
	private UnityAction<float> targetFrChangeHandler;

	private void Awake() {
		Debug.Assert(displaySettingsManager != null, "No display manager set for Display Settings menu.");
		Debug.Assert(resolutionDropdown != null, "No resolution dropdown set for Display Settings menu.");
		Debug.Assert(fullscreenModeDropdown != null, "No fullscreen mode dropdown set for Display Settings menu.");
		Debug.Assert(vsyncToggle != null, "No vsync toggle set for Display Settings menu.");
		Debug.Assert(targetFramerateSlider != null, "No target framerate slider set for Display Settings menu.");

		resolutionChangeHandler = value => displaySettingsManager.SetResolution(resolutionDropdown.options[value].text);
		fsModeChangeHandler = value => displaySettingsManager.SetFsMode(fullscreenModeDropdown.options[value].text);
		vsyncChangeHandler = value => displaySettingsManager.SetVsync(value);
		targetFrChangeHandler = value => displaySettingsManager.SetTargetFramerate(Mathf.RoundToInt(value));
	}

	private void OnEnable() {
		PopulateControls();
		
		resolutionDropdown.onValueChanged.AddListener(resolutionChangeHandler);
		fullscreenModeDropdown.onValueChanged.AddListener(fsModeChangeHandler);
		vsyncToggle.onValueChanged.AddListener(vsyncChangeHandler);
		targetFramerateSlider.onValueChanged.AddListener(targetFrChangeHandler);
	}

	private void OnDisable() {
		resolutionDropdown.onValueChanged.RemoveListener(resolutionChangeHandler);
		fullscreenModeDropdown.onValueChanged.RemoveListener(fsModeChangeHandler);
		vsyncToggle.onValueChanged.RemoveListener(vsyncChangeHandler);
		targetFramerateSlider.onValueChanged.RemoveListener(targetFrChangeHandler);
	}

	private void PopulateControls() {
		var resolutionOptions = new List<TMP_Dropdown.OptionData>();
		var currentRes = displaySettingsManager.CurrentResolution;
		var i = 0;
		var target = 0;
		foreach (var label in displaySettingsManager.SupportedResolutionLabels) {
			resolutionOptions.Add(new TMP_Dropdown.OptionData(label));
			if (currentRes == label) target = i;
			i++;
		}
		resolutionDropdown.options = resolutionOptions;
		resolutionDropdown.value = target;

		var fsModeOptions = new List<TMP_Dropdown.OptionData>();
		var currentFsMode = displaySettingsManager.CurrentFullscreenMode;
		i = target = 0;
		foreach (var mode in displaySettingsManager.SupportedFullscreenModes) {
			fsModeOptions.Add(new TMP_Dropdown.OptionData(mode));
			if (i == currentFsMode) target = i;
			i++;
		}
		fullscreenModeDropdown.options = fsModeOptions;
		fullscreenModeDropdown.value = target;

		vsyncToggle.isOn = displaySettingsManager.VsyncEnabled;
	}
}
}
