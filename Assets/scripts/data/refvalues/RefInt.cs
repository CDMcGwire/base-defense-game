using UnityEngine;

namespace data.refvalues {
[CreateAssetMenu(fileName = "ref-int", menuName = "Reference Value/Int", order = 1)]
public class RefInt : RefValue<int> {
	public override void Coerce(string text) {
		if (int.TryParse(text, out var value))
			Value.Current = value;
	}}
}