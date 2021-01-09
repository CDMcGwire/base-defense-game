using UnityEngine;

namespace data.refvalues {
[CreateAssetMenu(fileName = "ref-long", menuName = "Reference Value/Long", order = 1)]
public class RefLong : RefValue<long> {
	public override void Coerce(string text) {
		if (long.TryParse(text, out var value))
			Value.Current = value;
	}
}
}