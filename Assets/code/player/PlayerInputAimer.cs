using UnityEngine;
using UnityEngine.InputSystem;

namespace player {
public class PlayerInputAimer : MonoBehaviour {
	private const float MouseBuffer = 0.01f;

	public void Awake() {
		Debug.Assert(rotationRoot != null, "Targeting component does not have a managed transform.");
	}

	public void Target(InputAction.CallbackContext context) {
		var inputVector = context.ReadValue<Vector2>();
		var mousePos = (Vector2) refCamera.ScreenToWorldPoint(inputVector);

		if (aimPoint != null) AimFromPoint(mousePos);
		else AimFromRoot(mousePos);
	}

	private void AimFromRoot(Vector2 mousePos) {
		var pivot = (Vector2) rotationRoot.position;
		var forward = (Vector2) rotationRoot.right;
		var pivotToMouse = (mousePos - pivot).normalized;
		var aimDelta = Quaternion.FromToRotation(forward, pivotToMouse.normalized);
		rotationRoot.rotation *= aimDelta;
	}

	private void AimFromPoint(Vector2 mousePos) {
		var outer = (Vector2) aimPoint.position;
		var pivot = (Vector2) rotationRoot.position;

		var outerToPivot = pivot - outer;
		var pivotToMouse = mousePos - pivot;

		var a = pivotToMouse.magnitude;
		var b = outerToPivot.magnitude;

		if (a > b + MouseBuffer) {
			var A = Vector2.Angle(outerToPivot, aimPoint.right) * Mathf.Deg2Rad;
			var B = Mathf.Asin(b * Mathf.Sin(A) / a);

			var currentC = Vector2.Angle(pivotToMouse, outerToPivot * -1) * Mathf.Deg2Rad;
			var targetC = Mathf.PI - (A + B);
			var deltaC = (currentC - targetC) * Mathf.Rad2Deg;

			rotationRoot.Rotate(Vector3.forward, deltaC);
		}
		else {
			// Place the aim point in line with the vector from the pivot to the mouse.
			rotationRoot.rotation *= Quaternion.FromToRotation(outerToPivot * -1, pivotToMouse);
		}
	}

#pragma warning disable 0649
	[SerializeField] private Camera refCamera;
	[SerializeField] private Transform rotationRoot;
	[SerializeField] private Transform aimPoint;
#pragma warning restore 0649
}
}