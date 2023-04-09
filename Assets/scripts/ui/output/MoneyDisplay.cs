using data.refvalues;
using TMPro;
using UnityEngine;

namespace ui.output {
public class MoneyDisplay : RefValueObserver<long> {
#pragma warning disable 0649
	[SerializeField] private RefValue<long> money;
	[SerializeField] private TMP_Text textDisplay;
#pragma warning restore 0649

	protected override RefValue<long> Reference => money;

	protected override void OnEnableSub(long initialValue)
		=> OnValueChanged(initialValue);

	protected override void OnValueChanged(long current)
		=> textDisplay.text = $"${current:N0}";
}
}