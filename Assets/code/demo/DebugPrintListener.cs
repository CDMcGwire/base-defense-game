using UnityEngine;

namespace demo {
[CreateAssetMenu(fileName = "debug-print-listener", menuName = "Debug/PrintListener")]
public class DebugListener : ScriptableObject {
	public void Print(string value) => Debug.Log(value);

	public void Timestamp(string context) => Debug.Log($"C:{context} T:{Time.realtimeSinceStartup} F:{Time.frameCount}");
}
}
