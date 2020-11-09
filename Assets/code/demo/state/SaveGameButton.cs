using data.game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace demo.state {
public class SaveGameButton : MonoBehaviour {
	public SaveManagerService saveManager;
	public Button button;
	public TMP_InputField nameEntry;

	private UnityAction buttonAction;

	private void OnEnable() {
		buttonAction = () => saveManager.SaveGame(nameEntry.text);
		button.onClick.AddListener(buttonAction);
	}

	private void OnDisable() {
		button.onClick.RemoveListener(buttonAction);
	}
}
}
