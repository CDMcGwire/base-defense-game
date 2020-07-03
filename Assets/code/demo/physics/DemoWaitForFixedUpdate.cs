using System.Collections;

using UnityEngine;

namespace demo.physics {
public class DemoWaitForFixedUpdate : MonoBehaviour {
	private long collisions;
	private long updates;

	private void FixedUpdate() {
		updates++;
		Debug.Log($"Fixed Update Frame: {updates}; Collision Count = {collisions}");
	}

	private void OnCollisionEnter(Collision other) {
		StartCoroutine(AfterPhysics());
	}

	private IEnumerator AfterPhysics() {
		collisions++;
		Debug.Log($"Collision On Frame: {updates}; Collision Count = {collisions}");
		yield return new WaitForFixedUpdate();
		Debug.Log($"Resolved On Frame: {updates}; Collision Count = {collisions}");
		enabled = false;
	}
}
}