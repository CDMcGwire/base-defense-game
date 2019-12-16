using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ActiveMenuComponent : MonoBehaviour, IActiveMenu {
	[SerializeField] private bool hideOnDeactivate = true;
	[SerializeField] [Range(0.0f, 1.0f)] private float inactiveAlpha = 1.0f;
	[SerializeField] private bool blockWhenInactive = true;

	private CanvasGroup canvasGroup;
	private float originalAlpha = 1.0f;

	private void Awake() {
		canvasGroup = GetComponent<CanvasGroup>();
		originalAlpha = canvasGroup.alpha;
	}

	public void Activate() {
		if (!gameObject.activeSelf) gameObject.SetActive(true);

		canvasGroup.alpha = originalAlpha;
		canvasGroup.blocksRaycasts = true;
		canvasGroup.interactable = true;
	}

	public void Deactivate() {
		if (hideOnDeactivate) gameObject.SetActive(false);

		canvasGroup.alpha = inactiveAlpha;
		canvasGroup.blocksRaycasts = blockWhenInactive;
		canvasGroup.interactable = false;
	}
}