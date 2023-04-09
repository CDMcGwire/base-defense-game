using UnityEngine;
using UnityEngine.InputSystem;

namespace demo {
public class DebugPrintComponent : MonoBehaviour {
	public InputAction inputAction;

	public void Start() {
		inputAction.Enable();
	}

	public void Update() {
		if (inputAction.triggered) Timestamp("Direct Polling");
	}

	public void Print(string value) {
		Debug.Log(value);
	}

	public void Timestamp(string context) {
		Debug.Log($"C:{context} T:{Time.realtimeSinceStartup} F:{Time.frameCount}");
	}
}
}