using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace data.reactive {
[Serializable]
public class RxList<T> : RxVal, IList<T>, IRxReadonlyList<T> {
#pragma warning disable 0649
	[SerializeField] private List<T> list;
#pragma warning restore 0649

	public event Action<IReadOnlyList<T>> OnRestructure;
	public event Action<T, int> OnItemAdded;
	public event Action<T, int> OnItemRemoved;
	public event Action<T, T, int> OnItemChanged;
	public event Action<int> OnCountChanged;

	public int Count => list.Count;
	public bool IsReadOnly => (list as IList<T>).IsReadOnly;

	public RxList()
		=> list = new List<T>();

	public RxList(int capacity)
		=> list = new List<T>(capacity);

	public RxList(List<T> list)
		=> this.list = list ?? throw new ArgumentNullException(nameof(list));

	public T this[int index] {
		get => list[index];
		set {
			var previous = list[index];
			list[index] = value;
			OnItemChanged?.Invoke(previous, list[index], index);
		}
	}

	public void Add(T item) {
		list.Add(item);
		OnItemAdded?.Invoke(item, Count);
		OnCountChanged?.Invoke(list.Count);
	}

	public void AddRange(IEnumerable<T> collection) {
		list.AddRange(collection);
		OnRestructure?.Invoke(this);
		OnCountChanged?.Invoke(list.Count);
	}

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
		OnRestructure?.Invoke(this);
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

	public void Clear() {
		list.Clear();
		OnRestructure?.Invoke(this);
		OnCountChanged?.Invoke(0);
	}

	public bool Contains(T item) => list.Contains(item);

	public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

	public int IndexOf(T item) => list.IndexOf(item);

	public void Insert(int index, T item) {
		list.Insert(index, item);
		OnItemAdded?.Invoke(item, index);
		OnCountChanged?.Invoke(list.Count);
	}

	public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

	public bool Remove(T item) {
		var index = list.IndexOf(item);
		if (index < 0) return false;
		list.RemoveAt(index);
		OnItemRemoved?.Invoke(item, index);
		OnCountChanged?.Invoke(list.Count);
		return true;
	}

	public void RemoveAt(int index) {
		var item = list[index];
		list.RemoveAt(index);
		OnItemRemoved?.Invoke(item, index);
		OnCountChanged?.Invoke(list.Count);
	}

	public void RemoveLast() => RemoveAt(list.Count - 1);

	public void Repopulate(IEnumerable<T> collection) {
		list.Clear();
		list.AddRange(collection);
		OnRestructure?.Invoke(this);
		OnCountChanged?.Invoke(list.Count);
	}

	public void SizeTo(int count) {
		while (list.Count < count)
			list.Add(default);
		while (list.Count > count)
			list.RemoveAt(list.Count - 1);
		OnRestructure?.Invoke(this);
		OnCountChanged?.Invoke(count);
	}
}
}