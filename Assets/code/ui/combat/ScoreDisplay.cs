using data.refvalues;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace ui.combat {
public class ScoreDisplay : RefValueObserver<long> {
#pragma warning disable 0649
	[SerializeField] private RefValue<long> playerScore;
	[SerializeField] private TMP_Text text;
#pragma warning restore 0649

	protected override RefValue<long> Reference => playerScore;

	protected override void OnEnableSub(long initialValue)
		=> UpdateScoreDisplay(initialValue);

	[UsedImplicitly]
	public void UpdateScoreDisplay(long value)
		=> text.text = $"{value:00,000,000,000,000}";

	protected override void OnValueChanged(long current)
		=> UpdateScoreDisplay(current);
}
}
