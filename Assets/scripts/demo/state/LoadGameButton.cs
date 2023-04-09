using System.IO;
using data.game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace demo.state {
public class LoadGameButton : MonoBehaviour {
	public SaveManagerService saveManager;
	public Button button;
	public TMP_InputField nameEntry;

	private UnityAction buttonAction;

	private void OnEnable() {
		buttonAction = async () => {
			await saveManager.LoadGame(Path.Combine(Application.persistentDataPath, "game-saves", nameEntry.text + ".save"));
		};
		button.onClick.AddListener(buttonAction);
	}

	private void OnDisable() {
		button.onClick.RemoveListener(buttonAction);
	}
}
}
