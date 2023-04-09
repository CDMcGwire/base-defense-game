using System;
using UnityEngine;

namespace data.reactive {
/// <summary>
/// The Reactive constant is a utility for introducing static values to
/// reactive expressions.
/// </summary>
/// <typeparam name="T">The type of the value to build.</typeparam>
[Serializable]
public struct RxConst<T> : IRxReadonlyVal<T> {
	[SerializeField] private T current;

	public T Current => current;

// Should look into hoisting this into another interface, but I don't think I can. 
#pragma warning disable 0067
	public event Action<T> OnChanged;
#pragma warning restore 0067

	public RxConst(T value) : this()
		=> current = value ?? throw new ArgumentNullException(nameof(value));

	public void Bind(Action<T> action)
		=> action.Invoke(Current);
}

public static class RxConst {
	public static RxConst<bool> True = new(true);
	public static RxConst<bool> False = new(false);
	public static RxConst<int> Zero = new(0);
	public static RxConst<long> ZeroL = new(0);
}
}