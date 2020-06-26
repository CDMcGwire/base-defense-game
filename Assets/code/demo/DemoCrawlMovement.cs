using System;
using UnityEngine;

namespace demo {
public class DemoCrawlMovement : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private float speed = 1f;
	[SerializeField] private float pipHeight = 0.1f;
	[SerializeField] private float pipRate = 20.0f;
	[SerializeField] private bool airborne;
#pragma warning restore 0649

	private float time = 0;
	private float initialY = 0;

	private void Start() {
		initialY = transform.position.y;
	}

	private void Update() {
		time += Time.deltaTime;
		var position = transform.position;
		var newX = position.x - (speed * Time.deltaTime);
		var scaledPipTime = time * pipRate;
		var yOffset = airborne ? Math.Sin(scaledPipTime) : Math.Abs(Math.Sin(scaledPipTime));
		var newY = initialY + yOffset * pipHeight;
		transform.position = new Vector3(newX, (float)newY);
	}
}
}