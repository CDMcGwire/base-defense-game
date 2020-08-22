using UnityEngine;

namespace refvalues {
public abstract class RefValueObserver<T> : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private RefValue<T> observedValue;
#pragma warning restore 0649

	private void OnEnable() {
		observedValue.OnValueChanged += OnValueChanged;
	}

	private void OnDisable() {
		observedValue.OnValueChanged -= OnValueChanged;
	}

	protected abstract void OnValueChanged(T previous, T current);
}
}
