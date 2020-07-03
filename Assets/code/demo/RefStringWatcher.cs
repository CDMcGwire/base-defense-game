using UnityEngine;
using UnityEngine.UI;

namespace demo {
public class RefStringWatcher : MonoBehaviour {
	public Text textElement;
	public RefString value;

	public void Update() {
		textElement.text = value.Current;
	}
}
}