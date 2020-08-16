using UnityEngine;

namespace managers {
[CreateAssetMenu(fileName = "audio-manager", menuName = "Settings/Audio Manager")]
public class AudioSettingsManager : ScriptableObject {
	private const string Pref_Vol_Master = "pref-vol-master";
	private const string Pref_Mute_Master = "pref-mute-master";

	private void Awake() {
		if (PlayerPrefs.HasKey(Pref_Vol_Master))
			GameVolume = PlayerPrefs.GetFloat(Pref_Vol_Master);
	}

	public float GameVolume {
		get => AudioListener.volume;
		set {
			var clampedValue = Mathf.Clamp01(value);
			if (MutedMaster)
				AudioListener.volume = 0;
			else
				AudioListener.volume = clampedValue;
			PlayerPrefs.SetFloat(Pref_Vol_Master, clampedValue);
		}
	}

	public bool MutedMaster {
		get => PlayerPrefs.GetInt(Pref_Mute_Master, 0) >= 1;
		set {
			if (value) AudioListener.volume = 0;
			PlayerPrefs.SetInt(Pref_Mute_Master, value ? 1 : 0);
		}
	}
}
}
