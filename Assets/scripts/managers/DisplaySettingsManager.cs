using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace managers {
[CreateAssetMenu(fileName = "display-manager", menuName = "Settings/Display Manager")]
public class DisplaySettingsManager : ScriptableObject {
	// TODO: Finish updating to scriptable object based system and use implicit Pref initialization
	private const string Pref_Res = "d-res";
	private const string Pref_Res_FsMode = "d-res-fsmode";
	private const string Pref_Fr_Target = "d-fr-target";
	private const string Pref_Fr_Vsync = "d-fr-vsync";

	private readonly Dictionary<string, Resolution> resolutions = new();
	private readonly Dictionary<string, FullScreenMode> fsModeLabels = new() {
		{"Fullscreen Window", FullScreenMode.FullScreenWindow},
		{"Exclusive Fullscreen", FullScreenMode.ExclusiveFullScreen},
		{"Standard Window", FullScreenMode.Windowed},
	};
	private readonly SortedList<Resolution, string> sortedResolutions = new(new ResolutionComparer());

	public IEnumerable<string> SupportedResolutionLabels => sortedResolutions.Values;
	public string CurrentResolution => ToResLabel(Screen.currentResolution);
	public IEnumerable<string> SupportedFullscreenModes => fsModeLabels.Keys;
	public int CurrentFullscreenMode => (int) Screen.fullScreenMode;
	public bool VsyncEnabled => QualitySettings.vSyncCount > 0;

	private void Awake() {
		InitializeSupportedDisplayOptions();
		InitializeResolutionSettings();
		InitializeFramerateSettings();
	}

	private void InitializeSupportedDisplayOptions() {
		resolutions.Clear();
		foreach (var resolution in Screen.resolutions) {
			var resLabel = ToResLabel(resolution);
			resolutions[resLabel] = resolution;
			sortedResolutions.Add(resolution, resLabel);
		}
	}

	private void InitializeResolutionSettings() {
		// If no settings are found, initialize player prefs with the defaults.
		var resLabel = PlayerPrefs.HasKey(Pref_Res) 
			? PlayerPrefs.GetString(Pref_Res) 
			: CurrentResolution;
		var prefRes = Screen.currentResolution;
		if (!resolutions.ContainsKey(resLabel))
			Debug.LogWarningFormat("Player's preferred resolution of [{0}] is not supported. Using default.", resLabel);
		else prefRes = resolutions[resLabel];

		var fsModeIndex = PlayerPrefs.HasKey(Pref_Res_FsMode) 
			? PlayerPrefs.GetInt(Pref_Res_FsMode) 
			: CurrentFullscreenMode;
		var fsMode = Screen.fullScreenMode;
		if (!Enum.IsDefined(typeof(FullScreenMode), fsModeIndex))
			Debug.LogWarningFormat("Player's fullscreen mode index value of [{0}] is not valid. Using default.", fsModeIndex);
		else fsMode = (FullScreenMode) fsModeIndex;

		Screen.SetResolution(
			prefRes.width,
			prefRes.height,
			fsMode,
			prefRes.refreshRate
		);
	}

	private void InitializeFramerateSettings() {
		if (PlayerPrefs.HasKey(Pref_Fr_Target))
			Application.targetFrameRate = Math.Max(PlayerPrefs.GetInt(Pref_Fr_Target), -1);
		if (PlayerPrefs.HasKey(Pref_Fr_Vsync))
			QualitySettings.vSyncCount = Math.Min(Math.Max(0, PlayerPrefs.GetInt(Pref_Fr_Vsync)), 4);
	}

	[UsedImplicitly]
	public void SetResolution(string value) {
		if (!resolutions.ContainsKey(value)) {
			Debug.LogErrorFormat("Tried to set an unsupported resolution [{0}].", value);
			return;
		}
		var chosenRes = resolutions[value];
		Screen.SetResolution(
			chosenRes.width,
			chosenRes.height,
			Screen.fullScreenMode,
			chosenRes.refreshRate
		);
		Debug.LogFormat(
			"Set Resolution to {0} x {1} at {2} with mode as {3}",
			chosenRes.width,
			chosenRes.height,
			chosenRes.refreshRate,
			Screen.fullScreenMode.ToString()
		);
		PlayerPrefs.SetString(Pref_Res, value);
	}

	[UsedImplicitly]
	public void SetFsMode(string value) {
		Debug.LogFormat("Setting FS mode to {0}", value);
		if (!fsModeLabels.ContainsKey(value)) {
			Debug.LogErrorFormat("Tried to set an unsupported FullScreen Mode [{0}].", value);
			return;
		}
		var fsMode = fsModeLabels[value];
		var currentRes = Screen.currentResolution;
		Screen.SetResolution(currentRes.width, currentRes.height, fsMode, currentRes.refreshRate);
		Debug.LogFormat(
			"Set Resolution to {0} x {1} at {2} with mode as {3}",
			currentRes.width,
			currentRes.height,
			currentRes.refreshRate,
			fsMode.ToString()
		);
		PlayerPrefs.SetInt(Pref_Res_FsMode, (int) fsMode);
	}

	[UsedImplicitly]
	public void SetVsync(bool value) {
		QualitySettings.vSyncCount = value ? 1 : 0;
		PlayerPrefs.SetInt(Pref_Fr_Vsync, QualitySettings.vSyncCount);
	}

	[UsedImplicitly]
	public void SetTargetFramerate(int value) {
		// TODO: Reflect the 30 fps minimum in the settings display
		var clampedValue = Math.Max(-1, value);
		Application.targetFrameRate = clampedValue < 30 ? -1 : clampedValue;
		PlayerPrefs.SetInt(Pref_Fr_Target, Application.targetFrameRate);
	}

	private static string ToResLabel(Resolution res) => $"{res.width} x {res.height} at {res.refreshRate}Hz";

	private class ResolutionComparer : IComparer<Resolution> {
		public int Compare(Resolution x, Resolution y) {
			var result = y.width.CompareTo(x.width);
			if (result != 0) return result;

			result = y.height.CompareTo(x.height);
			if (result != 0) return result;

			return y.refreshRate.CompareTo(x.refreshRate);
		}
	}
}
}
