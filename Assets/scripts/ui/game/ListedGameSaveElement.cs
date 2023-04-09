using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ui.game {
public class ListedGameSaveElement : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private Button button;
	[SerializeField] private TMP_Text nameDisplay;
	[SerializeField] private TMP_Text dateDisplay;
#pragma warning restore 0649

	public Button Button => button;
	public TMP_Text NameDisplay => nameDisplay;
	public TMP_Text DateDisplay => dateDisplay;
}
}
