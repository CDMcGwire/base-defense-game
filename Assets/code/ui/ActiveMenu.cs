using UnityEngine;

namespace ui {
[RequireComponent(typeof(CanvasGroup))]
public class ActiveMenu : MonoBehaviour {
	[SerializeField] private bool hideOnDeactivate = true;
	[SerializeField] [Range(0.0f, 1.0f)] private float inactiveAlpha = 1.0f;
	[SerializeField] private bool blockWhenInactive = true;

	private Object claimer;
	private CanvasGroup canvasGroup;
	private float originalAlpha = 1.0f;

	private void Awake() {
		canvasGroup = GetComponent<CanvasGroup>();
		originalAlpha = canvasGroup.alpha;
	}

	public bool Activate(Object newClaimer) {
		if (claimer != null) return false;
		claimer = newClaimer;

		if (!gameObject.activeSelf) gameObject.SetActive(true);

		canvasGroup.alpha = originalAlpha;
		canvasGroup.blocksRaycasts = true;
		canvasGroup.interactable = true;
		return true;
	}

	public bool Deactivate(Object oldClaimer) {
		if (claimer != null && !ReferenceEquals(claimer, oldClaimer)) return false;
		if (hideOnDeactivate) gameObject.SetActive(false);

		canvasGroup.alpha = inactiveAlpha;
		canvasGroup.blocksRaycasts = blockWhenInactive;
		canvasGroup.interactable = false;
		return true;
	}
}
}