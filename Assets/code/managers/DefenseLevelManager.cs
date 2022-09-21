using System;

using UnityEngine;
using UnityEngine.Events;

namespace managers {
public class DefenseLevelManager : MonoBehaviour {
	public UnityEvent OnDayEnd => onDayEnd;

	public float DayTimer { get; set; }

	public float NormalizedTime => DayTimer / dayLength;

	private void OnValidate() {
		if (Math.Abs(dayLength) < 0.01) dayLength = 0.01f;
	}

	private void Update() {
		DayTimer += Time.deltaTime;
		if (DayTimer >= dayLength) OnDayEnd.Invoke();
	}
#pragma warning disable 0649
	[SerializeField] private float dayLength = 120f;
	[SerializeField] private UnityEvent onDayEnd = new();
#pragma warning restore 0649
}
}