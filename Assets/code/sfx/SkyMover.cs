using System;
using managers;
using UnityEngine;

namespace sfx {
public class SkyMover : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private DefenseLevelManager levelManager;
	[SerializeField] private Transform skyTransform;
	[SerializeField] private Transform endpoint;
#pragma warning restore 0649

	private Vector3 origin;

	private void OnEnable() {
		origin = skyTransform.position;
		UpdateSkyPosition();
	}

	private void OnDisable() => skyTransform.position = origin;

	private void Update() => UpdateSkyPosition();

	private void UpdateSkyPosition()
		=> skyTransform.position = Vector3.Lerp(
			origin,
			endpoint.position,
			levelManager.NormalizedTime
		);
}
}