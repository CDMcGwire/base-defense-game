using data.refvalues;
using TMPro;
using UnityEngine;

namespace ui.output {
public class RefLongDisplay : RefValueObserver<long> {
#pragma warning disable 0649
	[SerializeField] private RefValue<long> reference;
	[SerializeField] private TMP_Text display;
#pragma warning restore 0649
	
	protected override RefValue<long> Reference => reference;

	protected override void OnValueChanged(long current)
		=> display.text = $"{current:N}";
}
}
