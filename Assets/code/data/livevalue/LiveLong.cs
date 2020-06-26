using System;
using UnityEngine;

namespace data.livevalue {
/// <summary>
/// A mutable long that stores its initial value.
/// </summary>
[Serializable]
public struct LiveLong : ISerializationCallbackReceiver {
	[SerializeField] private long initial;
	private long? current;

	public LiveLong(long value) {
		current = initial = value;
	}

	public LiveLong(long initial, long? current) {
		this.initial = initial;
		this.current = current;
	}

	public long Initial => initial;
	public long Current => current ?? initial;

	public static LiveLong operator +(LiveLong value)
		=> value;

	public static LiveLong operator -(LiveLong value)
		=> new LiveLong(value.initial, -value.Current);

	public static LiveLong operator +(LiveLong lhs, LiveLong rhs)
		=> new LiveLong(lhs.initial + rhs.initial, lhs.Current + rhs.Current);

	public static LiveLong operator -(LiveLong lhs, LiveLong rhs)
		=> lhs + (-rhs);

	public static LiveLong operator +(LiveLong lhs, long rhs)
		=> new LiveLong(lhs.initial, lhs.Current + rhs);

	public static LiveLong operator -(LiveLong lhs, long rhs)
		=> lhs + (-rhs);

	public static LiveLong operator *(LiveLong lhs, LiveLong rhs)
		=> new LiveLong(lhs.initial * rhs.initial, lhs.Current * rhs.Current);

	public static LiveLong operator /(LiveLong lhs, LiveLong rhs)
		=> new LiveLong(lhs.initial / rhs.initial, lhs.Current / rhs.Current);

	public static LiveLong operator *(LiveLong lhs, long rhs)
		=> new LiveLong(lhs.initial, lhs.Current * rhs);

	public static LiveLong operator /(LiveLong lhs, long rhs)
		=> new LiveLong(lhs.initial, lhs.Current / rhs);

	public void OnBeforeSerialize() { }

	public void OnAfterDeserialize() => current = initial;
}
}