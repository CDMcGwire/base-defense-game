using System;

namespace data.reactive {
public struct RxAction<T1> {
	private readonly Action<T1> action;
	
	private IRxReadonlyVal<T1> rxVal;
	
	public RxAction(Action<T1> action) : this() => this.action = action;

	public void Attach(IRxReadonlyVal<T1> value) {
		rxVal = value ?? throw new ArgumentNullException(nameof(value));
		rxVal.OnChanged += action;
		action.Invoke(rxVal.Current);
	}

	public void Detach() {
		rxVal.OnChanged -= action;
		rxVal = null;
	}
}

public class RxAction<T1, T2> {
	private readonly Action<T1, T2> action;
	
	private IRxReadonlyVal<T1> rxVal1;
	private IRxReadonlyVal<T2> rxVal2;
	
	public RxAction(Action<T1, T2> action) => this.action = action;

	public void Attach(IRxReadonlyVal<T1> val1, IRxReadonlyVal<T2> val2) {
		rxVal1 = val1 ?? throw new ArgumentNullException(nameof(val1));
		rxVal2 = val2 ?? throw new ArgumentNullException(nameof(val2));
		rxVal1.OnChanged += OnRxVal1Change;
		rxVal2.OnChanged += OnRxVal2Change;
		action.Invoke(rxVal1.Current, rxVal2.Current);
	}

	public void Detach() {
		rxVal1.OnChanged -= OnRxVal1Change;
		rxVal2.OnChanged -= OnRxVal2Change;
		rxVal1 = null;
		rxVal2 = null;
	}

	private void OnRxVal1Change(T1 newVal1)
		=> React(newVal1, rxVal2.Current);

	private void OnRxVal2Change(T2 newVal2)
		=> React(rxVal1.Current, newVal2);

	private void React(T1 val1, T2 val2)
		=> action?.Invoke(val1, val2);
}
}