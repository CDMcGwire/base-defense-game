using UnityEngine;
using UnityEngine.UI;

namespace demo {
public class StatefulSoTextUpdater : MonoBehaviour {
	public StatefulScriptableObject data;
	public Text text;

	public void Refresh() {
		text.text = data.value;
	}
}
}