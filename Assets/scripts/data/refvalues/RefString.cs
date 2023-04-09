using UnityEngine;

namespace data.refvalues {
[CreateAssetMenu(fileName = "ref-string", menuName = "Reference Value/String", order = 1)]
public class RefString : RefValue<string> {
	public override void Coerce(string text)
		=> Value.Current = text;
}
}