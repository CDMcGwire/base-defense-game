using System;

using UnityEngine;

namespace data.livevalue {
/// <summary>A mutable double that stores its initial value.</summary>
[Serializable]
public struct LiveDouble : ISerializationCallbackReceiver {
	[SerializeField] private double initial;
	private double? current;

	public LiveDouble(double value) {
		current = initial = value;
	}

	public LiveDouble(double initial, double? current) {
		this.initial = initial;
		this.current = current;
	}

	public double Initial => initial;
	public double Current => current ?? initial;

	public static LiveDouble operator +(LiveDouble value) {
		return value;
	}

	public static LiveDouble operator -(LiveDouble value) {
		return new LiveDouble(value.initial, -value.Current);
	}

	public static LiveDouble operator +(LiveDouble lhs, LiveDouble rhs) {
		return new LiveDouble(lhs.initial + rhs.initial, lhs.Current + rhs.Current);
	}

	public static LiveDouble operator -(LiveDouble lhs, LiveDouble rhs) {
		return lhs + -rhs;
	}

	public static LiveDouble operator +(LiveDouble lhs, double rhs) {
		return new LiveDouble(lhs.initial, lhs.Current + rhs);
	}

	public static LiveDouble operator -(LiveDouble lhs, double rhs) {
		return lhs + -rhs;
	}

	public static LiveDouble operator *(LiveDouble lhs, LiveDouble rhs) {
		return new LiveDouble(lhs.initial * rhs.initial, lhs.Current * rhs.Current);
	}

	public static LiveDouble operator /(LiveDouble lhs, LiveDouble rhs) {
		return new LiveDouble(lhs.initial / rhs.initial, lhs.Current / rhs.Current);
	}

	public static LiveDouble operator *(LiveDouble lhs, double rhs) {
		return new LiveDouble(lhs.initial, lhs.Current * rhs);
	}

	public static LiveDouble operator /(LiveDouble lhs, double rhs) {
		return new LiveDouble(lhs.initial, lhs.Current / rhs);
	}

	public void OnBeforeSerialize() { }

	public void OnAfterDeserialize() {
		current = initial;
	}
}
}