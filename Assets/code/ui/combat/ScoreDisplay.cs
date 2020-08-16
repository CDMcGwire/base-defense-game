using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace ui.combat {
public class ScoreDisplay : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private TMP_Text text;
#pragma warning restore 0649

	[UsedImplicitly]
	public void UpdateScoreDisplay(long value) {
		text.text = $"{value:00,000,000,000,000}";
	}
}
}
