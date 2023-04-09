using System;

using UnityEngine;

namespace data.livevalue {
/// <summary>A mutable float that stores its initial value.</summary>
[Serializable]
public struct LiveFloat : ISerializationCallbackReceiver {
	[SerializeField] private float initial;
	private float? current;

	public LiveFloat(float value) {
		current = initial = value;
	}

	public LiveFloat(float initial, float? current) {
		this.initial = initial;
		this.current = current;
	}

	public float Initial => initial;
	public float Current => current ?? initial;

	public static LiveFloat operator +(LiveFloat value) {
		return value;
	}

	public static LiveFloat operator -(LiveFloat value) {
		return new LiveFloat(value.initial, -value.Current);
	}

	public static LiveFloat operator +(LiveFloat lhs, LiveFloat rhs) {
		return new LiveFloat(lhs.initial + rhs.initial, lhs.Current + rhs.Current);
	}

	public static LiveFloat operator -(LiveFloat lhs, LiveFloat rhs) {
		return lhs + -rhs;
	}

	public static LiveFloat operator +(LiveFloat lhs, float rhs) {
		return new LiveFloat(lhs.initial, lhs.Current + rhs);
	}

	public static LiveFloat operator -(LiveFloat lhs, float rhs) {
		return lhs + -rhs;
	}

	public static LiveFloat operator *(LiveFloat lhs, LiveFloat rhs) {
		return new LiveFloat(lhs.initial * rhs.initial, lhs.Current * rhs.Current);
	}

	public static LiveFloat operator /(LiveFloat lhs, LiveFloat rhs) {
		return new LiveFloat(lhs.initial / rhs.initial, lhs.Current / rhs.Current);
	}

	public static LiveFloat operator *(LiveFloat lhs, float rhs) {
		return new LiveFloat(lhs.initial, lhs.Current * rhs);
	}

	public static LiveFloat operator /(LiveFloat lhs, float rhs) {
		return new LiveFloat(lhs.initial, lhs.Current / rhs);
	}

	public void OnBeforeSerialize() { }

	public void OnAfterDeserialize() {
		current = initial;
	}
}
}