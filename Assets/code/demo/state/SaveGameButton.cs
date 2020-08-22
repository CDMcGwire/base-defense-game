using System;
using data.game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace demo.state {
public class SaveGameButton : MonoBehaviour {
	public GameStateSo gameState;
	public Button button;
	public TMP_InputField nameEntry;

	private UnityAction buttonAction;

	private void OnEnable() {
		buttonAction = () => gameState.SaveGame(nameEntry.text);
		button.onClick.AddListener(buttonAction);
	}

	private void OnDisable() {
		button.onClick.RemoveListener(buttonAction);
	}
}
}
