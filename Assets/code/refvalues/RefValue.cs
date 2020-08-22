using System;
using UnityEngine;

namespace refvalues {
[Serializable]
public abstract class RefValue : ScriptableObject {
	public abstract string SerializeCurrentValue();
	public abstract void LoadFromJson(string json);
}

public class RefValue<T> : RefValue, ISerializationCallbackReceiver {
#pragma warning disable 0649
	[SerializeField] private T value;
#pragma warning restore 0649

	private T current;

	public T Value {
		get => current;
		set {
			var last = current;
			current = value;
			OnValueChanged?.Invoke(last, value);
		}
	}

	public event Action<T, T> OnValueChanged;

	public void OnBeforeSerialize() { }

	public void OnAfterDeserialize() => current = value;

	public override string SerializeCurrentValue()
		=> JsonUtility.ToJson(new WrappedValue<T>(current));

	public override void LoadFromJson(string json) {
		var fromJson = JsonUtility.FromJson<WrappedValue<T>>(json);
		Value = fromJson.value;
	}
}

[Serializable]
public struct WrappedValue<T> {
	public T value;
	public WrappedValue(T value) => this.value = value;
}
}
