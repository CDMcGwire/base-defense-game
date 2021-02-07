using System;
using System.Collections;
using System.Collections.Generic;

namespace data.reactive {
public interface IRxReadonlyList<T> : IReadOnlyList<T> {
	event Action<IReadOnlyList<T>> OnRestructure;
	event Action<T, int> OnItemAdded;
	event Action<T, int> OnItemRemoved;
	event Action<T, T, int> OnItemChanged;
	event Action<int> OnCountChanged;

	void Bind(
		Action<IReadOnlyList<T>> restructureAction,
		Action<T, int> addAction,
		Action<T, int> removeAction,
		Action<T, T, int> changeAction
	);

	void Unbind(
		Action<IReadOnlyList<T>> restructureAction,
		Action<T, int> addAction,
		Action<T, int> removeAction,
		Action<T, T, int> changeAction
	);
}

public class RxListPassthrough<T> : IRxReadonlyList<T> {
	private IRxReadonlyList<T> source;

	public event Action<IReadOnlyList<T>> OnRestructure;
	public event Action<T, int> OnItemAdded;
	public event Action<T, int> OnItemRemoved;
	public event Action<T, T, int> OnItemChanged;
	public event Action<int> OnCountChanged;

	public int Count => source.Count;

	public RxListPassthrough() {
		source = Empty;
	}

	public T this[int index] => source[index];

	public void Attach(IRxReadonlyList<T> target) {
		if (source != Empty)
			Detach();
		source = target;
		source.OnRestructure += SourceRestructureAction;
		source.OnItemAdded += SourceItemAddedAction;
		source.OnItemRemoved += SourceItemRemovedAction;
		source.OnItemChanged += SourceItemChangedAction;
		source.OnCountChanged += SourceCountChangedAction;
	}

	public void Detach() {
		if (source == Empty)
			return;
		source.OnRestructure -= SourceRestructureAction;
		source.OnItemAdded -= SourceItemAddedAction;
		source.OnItemRemoved -= SourceItemRemovedAction;
		source.OnItemChanged -= SourceItemChangedAction;
		source.OnCountChanged -= SourceCountChangedAction;
		source = Empty;
	}

	private void SourceRestructureAction(IReadOnlyList<T> list)
		=> OnRestructure?.Invoke(list);

	private void SourceItemAddedAction(T value, int index)
		=> OnItemAdded?.Invoke(value, index);

	private void SourceItemRemovedAction(T value, int index)
		=> OnItemRemoved?.Invoke(value, index);

	private void SourceItemChangedAction(T previous, T current, int index)
		=> OnItemChanged?.Invoke(previous, current, index);

	private void SourceCountChangedAction(int count)
		=> OnCountChanged?.Invoke(count);

	public IEnumerator<T> GetEnumerator() => source.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public void Bind(
		Action<IReadOnlyList<T>> restructureAction,
		Action<T, int> addAction,
		Action<T, int> removeAction,
		Action<T, T, int> changeAction
	) {
		OnRestructure += restructureAction;
		OnItemAdded += addAction;
		OnItemRemoved += removeAction;
		OnItemChanged += changeAction;
		OnRestructure?.Invoke(source);
	}

	public void Unbind(
		Action<IReadOnlyList<T>> restructureAction,
		Action<T, int> addAction,
		Action<T, int> removeAction,
		Action<T, T, int> changeAction
	) {
		OnRestructure -= restructureAction;
		OnItemAdded -= addAction;
		OnItemRemoved -= removeAction;
		OnItemChanged -= changeAction;
	}

	private static IRxReadonlyList<T> Empty { get; } = new RxList<T>(0);
}
}