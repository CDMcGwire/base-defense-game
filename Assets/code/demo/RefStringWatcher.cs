using UnityEngine;
using UnityEngine.UI;

public class RefStringWatcher : MonoBehaviour {
	public RefString value;

	public Text textElement;

	public void Update() => textElement.text = value.Current;
}
