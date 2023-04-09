using System;
using UnityEngine;

namespace data.reactive {
[Serializable]
public class RxVal { }

[Serializable]
public class RxVal<T> : RxVal, IRxReadonlyVal<T> {
#pragma warning disable 0649
	[SerializeField] private T current;
#pragma warning restore 0649

	/// <summary>
	/// The current actual value of the observable. Assigning to this property
	/// will invoke an OnChange event.
	/// </summary>
	public T Current {
		get => current;
		set {
			current = value;
			OnChanged?.Invoke(current);
		}
	}

	public event Action<T> OnChanged;

	public RxVal() => current = default;

	public RxVal(T initial) => current = initial;

	public void Bind(Action<T> action) {
		OnChanged += action;
		action.Invoke(current);
	}
}
}