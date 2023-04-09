using System;

using combat.components;

using UnityEngine;
using UnityEngine.Events;

namespace ui.combat {
public class HealthSprite : MonoBehaviour {
	private UnityAction<DamageReport> handleDamageEvent;

	private float initialScale;

	private void Awake() {
		switch (scaleDirection) {
			case ScaleDirection.X:
				initialScale = mask.transform.localScale.x;
				break;
			case ScaleDirection.Y:
				initialScale = mask.transform.localScale.y;
				break;
			case ScaleDirection.Z:
				initialScale = mask.transform.localScale.z;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void OnEnable() {
		if (target == null) return;
		handleDamageEvent = HandleDamageEvent;
		target.OnDamaged.AddListener(handleDamageEvent);
		UpdateBar(target.CurrentHealth, target.MaxHealth);
	}

	private void OnDisable() {
		if (target == null) return;
		target.OnDamaged.RemoveListener(handleDamageEvent);
	}

	private void Start() {
		UpdateBar(target.CurrentHealth, target.MaxHealth);
	}

	private void UpdateBar(long current, long max) {
		var localScale = mask.transform.localScale;
		var relativeFill = CalcScale(current, max);

		switch (scaleDirection) {
			case ScaleDirection.X:
				localScale.x = relativeFill;
				break;
			case ScaleDirection.Y:
				localScale.y = relativeFill;
				break;
			case ScaleDirection.Z:
				localScale.z = relativeFill;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		mask.transform.localScale = localScale;
	}

	private float CalcScale(long current, long max) {
		return Mathf.Clamp(current / (float) max, 0, 1) * initialScale;
	}

	public void HandleDamageEvent(DamageReport report) {
		UpdateBar(report.currentHealth, report.maxHealth);
	}
#pragma warning disable 0649
	[SerializeField] private DamageComponent target;
	[SerializeField] private GameObject mask;
	[SerializeField] private ScaleDirection scaleDirection = ScaleDirection.Y;
#pragma warning restore 0649
}

[Serializable]
public enum ScaleDirection {
	X,
	Y,
	Z
}
}