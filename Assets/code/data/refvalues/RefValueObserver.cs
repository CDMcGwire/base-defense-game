using System;
using UnityEngine;

namespace data.refvalues {
public abstract class RefValueObserver<T> : MonoBehaviour {
	protected abstract RefValue<T> Reference { get; }
	protected T CurrentObservedValue => Reference.Value.Current;

	private void OnEnable() {
		if (Reference == null) {
			Debug.LogWarningFormat("A Reference Value Observer '{0}' is active but has no assigned value.", name);
			return;
		}
		var value = Reference.Value;
		if (value == null)
			throw new Exception($"Reference Value Observer {name} failed to subscribe to {Reference.name}");
		value.OnChanged += OnValueChanged;
		OnEnableSub(value.Current);
	}

	protected virtual void OnEnableSub(T initialValue) { }

	private void OnDisable() {
		if (Reference == null) return;
		var value = Reference.Value;
		if (value == null) return;
		value.OnChanged -= OnValueChanged;
		OnDisableSub(value.Current);
	}

	protected virtual void OnDisableSub(T finalValue) { }

	protected abstract void OnValueChanged(T previous, T current);
}
}