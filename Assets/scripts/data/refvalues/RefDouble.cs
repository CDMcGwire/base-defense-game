using UnityEngine;

namespace data.refvalues {
[CreateAssetMenu(fileName = "ref-double", menuName = "Reference Value/double", order = 1)]
public class RefDouble : RefValue<double> {
	public override void Coerce(string text) {
		if (double.TryParse(text, out var value))
			Value.Current = value;
	}
}
}