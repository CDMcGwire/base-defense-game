using System;
using UnityEngine;

namespace demo {
public class ScreenSizeReporter : MonoBehaviour {
	private void Start() {
		Debug.LogFormat("Start W = {0} - H = {1}", Screen.width, Screen.height);
	}

	private void Awake() {
		Debug.LogFormat("Awake W = {0} - H = {1}", Screen.width, Screen.height);
	}

	private void OnEnable() {
		Debug.LogFormat("Enable W = {0} - H = {1}", Screen.width, Screen.height);
	}

	private void Update() {
		Debug.LogFormat("Update W = {0} - H = {1}", Screen.width, Screen.height);
	}
}
}
