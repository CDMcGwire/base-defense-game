using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "event", menuName = "EventAssets/Simple", order = 1)]
public class RefEvent : ScriptableObject {
	private HashSet<RefEventListener> listeners = new HashSet<RefEventListener>();

	public void Add(RefEventListener listener) => _ = listeners.Add(listener);

	public void Remove(RefEventListener listener) => _ = listeners.Remove(listener);

	public void Trigger() {
		foreach (var listener in listeners) listener.Trigger();
	}

	public void OnDestroy() => listeners.Clear();
}

public class RefEvent<T> : ScriptableObject {
	private HashSet<RefEventListener<T>> listeners = new HashSet<RefEventListener<T>>();

	public void Add(RefEventListener<T> listener) => _ = listeners.Add(listener);

	public void Remove(RefEventListener<T> listener) => _ = listeners.Remove(listener);

	public void Trigger(T t) {
		foreach (var listener in listeners) listener.Trigger(t);
	}

	public void OnDestroy() => listeners.Clear();
}
