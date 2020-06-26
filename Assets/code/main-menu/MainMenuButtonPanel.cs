using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

/// <summary>
/// Convenience component to bring the buttons panel configuration up
/// to the top level of the hierarchy.
/// </summary>
public class MainMenuButtonPanel : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private Button newButton;
	[SerializeField] private ButtonClickedEvent onNew;
	[SerializeField] private Button loadButton;
	[SerializeField] private ButtonClickedEvent onLoad;
	[SerializeField] private Button settingsButton;
	[SerializeField] private ButtonClickedEvent onSettings;
#pragma warning restore 0649

	public void Start() {
		newButton.onClick = onNew;
		loadButton.onClick = onLoad;
		settingsButton.onClick = onSettings;
	}
}