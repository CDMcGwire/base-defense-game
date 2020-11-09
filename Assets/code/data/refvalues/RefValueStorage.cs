using System;
using System.Collections.Generic;
using UnityEngine;

namespace data.refvalues {
public class RefValueStorage : MonoBehaviour {
	private readonly Dictionary<string, Observable> values = new Dictionary<string, Observable>();

	public Observable<T> Lookup<T>(string key) {
		if (values.ContainsKey(key))
			return values[key] is Observable<T> observable
				? observable
				: null;
		return null;
	}

	public Observable<T> Publish<T>(string key, T value) {
		var observable = new Observable<T>(value);
		values[key] = observable;
		return observable;
	}
}

// TODO: Need to make this trigger a kill switch with some sort of "unsubscribe" operation which can be triggered on the
// TODO: Observer side.

[Serializable]
public class Observable { }

[Serializable]
public class Observable<T> : Observable {
	private T current;

	public T Current {
		get => current;
		set {
			var previous = current;
			current = value;
			OnChanged?.Invoke(previous, current);
		}
	}

	public event Action<T, T> OnChanged;

	public Observable(T initial) => current = initial;
}
}