using UnityEngine;

namespace camera {
public class OrthoCameraWidthLock : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private float widthInUnits = 32.2f;
#pragma warning restore 0649

	private Camera cam;

	private void Awake() => cam = GetComponent<Camera>();

	private void Update() {
		var hwAspect = (float) Screen.height / Screen.width;
		var targetSize = widthInUnits * hwAspect / 2;
		if (Mathf.Abs(targetSize - cam.orthographicSize) > 0.001)
			cam.orthographicSize = targetSize;
	}
}
}
