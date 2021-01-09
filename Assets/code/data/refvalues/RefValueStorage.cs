using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace data.refvalues {
public class RefValueStorage : MonoBehaviour {
	private readonly Dictionary<string, ReactiveVal> values = new Dictionary<string, ReactiveVal>();

	public ReactiveVal<T> Lookup<T>(string key) {
		if (values.ContainsKey(key))
			return values[key] is ReactiveVal<T> observable
				? observable
				: null;
		return null;
	}

	public ReactiveVal<T> Publish<T>(string key, T value) {
		var observable = new ReactiveVal<T>(value);
		values[key] = observable;
		return observable;
	}
}

[Serializable]
public class ReactiveVal { }

[Serializable]
public class ReactiveVal<T> : ReactiveVal, IReactiveReadVal<T> {
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
			var previous = current;
			current = value;
			OnChanged?.Invoke(previous, current);
		}
	}

	public event Action<T, T> OnChanged;

	public ReactiveVal() => current = default;

	public ReactiveVal(T initial) => current = initial;
}

[Serializable]
public class ReactiveList<T> : ReactiveVal, IList<T>, IReactiveReadList<T> {
#pragma warning disable 0649
	[SerializeField] private List<T> list;
#pragma warning restore 0649

	public event Action<T, T, int> OnItemChanged;
	public event Action<T, int> OnItemAdded;
	public event Action<T, int> OnItemRemoved;
	public event Action<int, int> OnCountChanged;
	public event Action<IList<T>> OnRestructure;

	public int Count => list.Count;
	public bool IsReadOnly => (list as IList<T>).IsReadOnly;

	public ReactiveList()
		=> list = new List<T>();

	public ReactiveList(int capacity)
		=> list = new List<T>(capacity);

	public ReactiveList(List<T> list)
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
		OnCountChanged?.Invoke(list.Count - 1, list.Count);
	}

	public void AddRange(IEnumerable<T> collection) {
		var oldCount = list.Count;
		list.AddRange(collection);
		OnRestructure?.Invoke(this);
		OnCountChanged?.Invoke(oldCount, list.Count);
	}

	public void Clear() {
		var oldCount = list.Count;
		list.Clear();
		OnRestructure?.Invoke(this);
		OnCountChanged?.Invoke(oldCount, 0);
	}

	public bool Contains(T item) => list.Contains(item);

	public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

	public int IndexOf(T item) => list.IndexOf(item);

	public void Insert(int index, T item) {
		list.Insert(index, item);
		OnItemAdded?.Invoke(item, index);
		OnCountChanged?.Invoke(list.Count - 1, list.Count);
	}

	public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

	public bool Remove(T item) {
		var index = list.IndexOf(item);
		if (index < 0) return false;
		list.RemoveAt(index);
		return true;
	}

	public void RemoveAt(int index) {
		var item = list[index];
		list.RemoveAt(index);
		OnItemRemoved?.Invoke(item, index);
		OnCountChanged?.Invoke(list.Count + 1, list.Count);
	}

	public void RemoveLast() => RemoveAt(list.Count - 1);

	public void Repopulate(IEnumerable<T> collection) {
		var oldCount = list.Count;
		list.Clear();
		list.AddRange(collection);
		OnRestructure?.Invoke(this);
		OnCountChanged?.Invoke(oldCount, list.Count);
	}

	public void SizeTo(int count) {
		while (list.Count < count)
			Add(default);
		while (list.Count > count)
			RemoveLast();
	}
}

/// <summary>
/// An interface to handle a ReactiveVal with for read-only access.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IReactiveReadVal<out T> {
	T Current { get; }
	event Action<T, T> OnChanged;
}

public interface IReactiveReadList<T> : IReadOnlyList<T> {
	event Action<T, T, int> OnItemChanged;
	event Action<T, int> OnItemAdded;
	event Action<T, int> OnItemRemoved;
	event Action<int, int> OnCountChanged;
	event Action<IList<T>> OnRestructure;
}
}