using UnityEngine;

namespace data.refvalues {
[CreateAssetMenu(fileName = "ref-bool", menuName = "Reference Value/Bool", order = 1)]
public class RefBool : RefValue<bool> {
	public override void Coerce(string text) {
		if (bool.TryParse(text, out var value))
			Value.Current = value;
	}
}
}