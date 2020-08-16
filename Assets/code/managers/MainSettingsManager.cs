using UnityEngine;

namespace managers {
[CreateAssetMenu(fileName = "player-prefs-manager", menuName = "Settings/Main Prefs Manager")]
public class MainSettingsManager : ScriptableObject {
	public void SaveSettings() => PlayerPrefs.Save();
}
}
