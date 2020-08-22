using refvalues;
using TMPro;
using UnityEngine;

namespace ui.output {
public class RefLongDisplay : RefValueObserver<long> {
#pragma warning disable 0649
	[SerializeField] private TMP_Text display;
#pragma warning restore 0649
	protected override void OnValueChanged(long previous, long current) {
		display.text = $"{current:N}";
	}
}
}
