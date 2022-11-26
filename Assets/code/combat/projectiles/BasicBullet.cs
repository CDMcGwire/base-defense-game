using UnityEngine;

namespace combat.projectiles {
public class BasicBullet : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private float speedInUps = 10.0f;
	[SerializeField] private float maximumRange = 40.0f;
#pragma warning restore 0649

	private Rigidbody2D rb;
	private Vector2 lastPos;
	private float distanceTravelled;

	private TrailRenderer trailRenderer;

	private void Awake() {
		var go = gameObject;
		rb = go.GetComponent<Rigidbody2D>();
		Debug.AssertFormat(
			rb != null,
			"BasicBullet [{0}] has been attached to an object with no rigidbody.",
			go.name
		);
		trailRenderer = go.GetComponent<TrailRenderer>();
	}

	private void OnEnable() {
		var targetVelocity = transform.right * speedInUps;
		rb.velocity = targetVelocity;
		lastPos = rb.position;
	}

	private void Update() {
		var currentPos = rb.position;
		distanceTravelled += Vector2.Distance(lastPos, currentPos);
		lastPos = currentPos;
		
		if (distanceTravelled >= maximumRange)
			gameObject.SetActive(false);
	}

	private void OnDisable() {
		distanceTravelled = 0;
		lastPos = Vector2.zero;
		if (!ReferenceEquals(trailRenderer, null))
			trailRenderer.Clear();
	}
}
}