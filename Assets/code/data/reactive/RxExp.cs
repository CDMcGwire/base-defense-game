using System;

namespace data.reactive {
/// <summary>
/// The Unary Reactive Expression provides a convenience tool for creating
/// chainable single input transformations on a Reactive value.
///
/// The reason for the "Attach/Detach" methods are so that the operation can be
/// "turned off" at-will and cleaned up for reuse, as it is common for UI
/// elements to be pooled or frequently deactivated. It is, however, expected
/// that the operation function will not change, only the source of its inputs
/// so only the input values are Attached and Detached in this manner.
/// </summary>
/// <typeparam name="T1">The first input type of the operation.</typeparam>
/// <typeparam name="TResult">The combined output.</typeparam>
public class RxExp<T1, TResult> : IRxReadonlyVal<TResult> {
	private IRxReadonlyVal<T1> rxVal;

	private readonly Func<T1, TResult> operation;
	public event Action<TResult> OnChanged;

	public TResult Current { get; private set; }

	public RxExp(Func<T1, TResult> operation)
		=> this.operation = operation ?? throw new ArgumentNullException(nameof(operation));

	/// <summary>
	/// Set the input of the expression, which will immediately update the
	/// current value of the expression based on the provided inputs and
	/// operation. Calling Attach with a new set of values will automatically
	/// detach the last set.
	/// </summary>
	/// <param name="val1">The reactive value or expression that provides the first input.</param>
	/// <exception cref="ArgumentNullException">Thrown if called with any null references.</exception>
	public void Attach(IRxReadonlyVal<T1> val1) {
		if (val1 == null) throw new ArgumentNullException(nameof(val1));
		if (rxVal != null)
			rxVal.OnChanged -= Recalculate;
		rxVal = val1;
		rxVal.OnChanged += Recalculate;
		Recalculate(rxVal.Current);
	}

	/// <summary>
	/// Remove all current inputs, stopping this expression from receiving events
	/// and unlinking their lifetimes with respect to the garbage collector.
	/// </summary>
	public void Detach() {
		if (rxVal == null) return;
		rxVal.OnChanged -= Recalculate;
		rxVal = null;
	}

	private void Recalculate(T1 newVal) {
		Current = operation.Invoke(newVal);
		OnChanged?.Invoke(Current);
	}

	public void Bind(Action<TResult> onChangeAction) {
		OnChanged += onChangeAction;
		onChangeAction.Invoke(Current);
	}
}

/// <summary>
/// The Binary Reactive operation is a convenience tool for synchronizing the
/// updates of two Reactive Values and providing a combined output when either
/// one updates. The combination method and expected output type are the same
/// for all listeners, as operations can be nested.
///
/// The reason for the "Attach/Detach" methods are so that the operation can be
/// "turned off" at-will and cleaned up for reuse, as it is common for UI
/// elements to be pooled or frequently deactivated. It is, however, expected
/// that the operation function will not change, only the source of its inputs
/// so only the input values are Attached and Detached in this manner.
/// </summary>
/// <typeparam name="T1">The first input type of the operation.</typeparam>
/// <typeparam name="T2">The second input type of the operation.</typeparam>
/// <typeparam name="TResult">The combined output.</typeparam>
public class RxExp<T1, T2, TResult> : IRxReadonlyVal<TResult> {
	private IRxReadonlyVal<T1> rxVal1;
	private IRxReadonlyVal<T2> rxVal2;

	private readonly Func<T1, T2, TResult> operation;
	public event Action<TResult> OnChanged;

	public TResult Current { get; private set; }

	public RxExp(Func<T1, T2, TResult> operation)
		=> this.operation = operation ?? throw new ArgumentNullException(nameof(operation));
	
	/// <summary>
	/// Set the input of the expression, which will immediately update the
	/// current value of the expression based on the provided inputs and
	/// operation. Calling Attach with a new set of values will automatically
	/// detach the last set.
	/// </summary>
	/// <param name="val1">The reactive value or expression that provides the first input.</param>
	/// <param name="val2">The reactive value or expression that provides the second input.</param>
	/// <exception cref="ArgumentNullException">Thrown if called with any null references.</exception>
	public void Attach(IRxReadonlyVal<T1> val1, IRxReadonlyVal<T2> val2) {
		if (val1 == null) throw new ArgumentNullException(nameof(val1));
		if (val2 == null) throw new ArgumentNullException(nameof(val2));
		
		if (rxVal1 != null)
			rxVal1.OnChanged -= OnRxVal1Change;
		rxVal1 = val1;
		if (rxVal2 != null)
			rxVal2.OnChanged -= OnRxVal2Change;
		rxVal2 = val2;
		
		rxVal1.OnChanged += OnRxVal1Change;
		rxVal2.OnChanged += OnRxVal2Change;
		Recalculate(rxVal1.Current, rxVal2.Current);
	}

	/// <summary>
	/// Remove all current inputs, stopping this expression from receiving events
	/// and unlinking their lifetimes with respect to the garbage collector.
	/// </summary>
	public void Detach() {
		rxVal1.OnChanged -= OnRxVal1Change;
		rxVal2.OnChanged -= OnRxVal2Change;
		rxVal1 = null;
		rxVal2 = null;
	}

	private void OnRxVal1Change(T1 newVal1)
		=> Recalculate(newVal1, rxVal2.Current);

	private void OnRxVal2Change(T2 newVal2)
		=> Recalculate(rxVal1.Current, newVal2);

	private void Recalculate(T1 val1, T2 val2) {
		Current = operation.Invoke(val1, val2);
		OnChanged?.Invoke(Current);
	}

	public void Bind(Action<TResult> onChangeAction) {
		OnChanged += onChangeAction;
		onChangeAction.Invoke(Current);
	}
}

/// <summary>
/// A static utility class that can be used to quickly build common, generic
/// expressions.
/// </summary>
/// <typeparam name="T">The input and output type of the resulting expressions.</typeparam>
public static class RxExp<T> {
	private static readonly Func<T, T> PassthroughFunc = value => value;
	public static RxExp<T, T> Passthrough => new(PassthroughFunc);
}
}