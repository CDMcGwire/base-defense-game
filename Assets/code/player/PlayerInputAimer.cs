using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace player {
public class PlayerInputAimer : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private Camera refCamera;
	[SerializeField] private Transform pivot;
	[SerializeField] private Transform target;
	[SerializeField] private Transform ikRoot;
	[SerializeField] private Transform ikHandle;
	[SerializeField] private bool ikAlignRotation;
	[SerializeField] private float ikMaxDist = 100f;
#pragma warning restore 0649

	private const float MouseBuffer = 0.01f;

	private bool IkEnabled
		=> ikRoot != null && ikHandle != null;

	private bool IkTargetNotOverlapped
		=> Vector2.Distance(ikRoot.position, ikHandle.position) > ikMaxDist;

	public Camera RefCamera {
		get => refCamera;
		set => refCamera = value;
	}

	public Transform Targeter {
		get => target;
		set => target = value;
	}

	public void Awake() {
		Debug.Assert(
			pivot != null,
			"Targeting component does not have a managed transform."
		);
	}

	[UsedImplicitly]
	public void Target(InputAction.CallbackContext context) {
		if (refCamera == null) return;
		var inputVector = context.ReadValue<Vector2>();
		var mousePos = (Vector2) refCamera.ScreenToWorldPoint(inputVector);

		if (IkEnabled) PositionIkHandle(mousePos);

		if (!IkEnabled || IkTargetNotOverlapped) {
			if (target != null) AimFromPoint(mousePos);
			else AimFromRoot(mousePos);
		}
	}

	private void AimFromRoot(Vector2 mousePos) {
		var pivotPos = (Vector2) pivot.position;
		var forward = (Vector2) pivot.right;
		var pivotToMouse = (mousePos - pivotPos).normalized;
		var aimDelta = Quaternion.FromToRotation(forward, pivotToMouse.normalized);
		pivot.rotation *= aimDelta;
	}

	private void AimFromPoint(Vector2 mousePos) {
		var outer = (Vector2) target.position;
		var pivotPos = (Vector2) pivot.position;

		var outerToPivot = pivotPos - outer;
		var pivotToMouse = mousePos - pivotPos;

		var a = pivotToMouse.magnitude;
		var b = outerToPivot.magnitude;

		if (a > b + MouseBuffer) {
			var A = Vector2.Angle(outerToPivot, target.right) * Mathf.Deg2Rad;
			var B = Mathf.Asin(b * Mathf.Sin(A) / a);

			var currentC = Vector2.Angle(pivotToMouse, outerToPivot * -1) * Mathf.Deg2Rad;
			var targetC = Mathf.PI - (A + B);
			var deltaC = (currentC - targetC) * Mathf.Rad2Deg;

			pivot.Rotate(Vector3.forward, IkEnabled ? -deltaC : deltaC);
		}
		else {
			// Place the aim point in line with the vector from the pivot to the mouse.
			pivot.rotation *= Quaternion.FromToRotation(outerToPivot * -1, pivotToMouse);
		}
	}

	private void PositionIkHandle(Vector2 mousePos) {
		if (Vector2.Distance(mousePos, ikRoot.position) > ikMaxDist) {
			var position = (Vector2) ikRoot.position;
			var mouseDir = (mousePos - position).normalized;
			ikHandle.position = position + mouseDir * ikMaxDist;
		}
		else ikHandle.position = mousePos;
		if (ikAlignRotation)
			ikHandle.right = (mousePos - (Vector2) ikRoot.position).normalized;
	}
}
}