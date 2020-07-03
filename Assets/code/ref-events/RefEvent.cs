using System.Collections.Generic;

using JetBrains.Annotations;

using UnityEngine;

[CreateAssetMenu(fileName = "event", menuName = "EventAssets/Simple", order = 1)]
public class RefEvent : ScriptableObject {
	private readonly HashSet<RefEventListener> listeners = new HashSet<RefEventListener>();
	private readonly List<RefEventListener> readyToRemove = new List<RefEventListener>();
	private readonly object syncObject = new object();
	private bool triggering;

	public void Add(RefEventListener listener) {
		_ = listeners.Add(listener);
	}

	public void Remove(RefEventListener listener) {
		if (triggering) readyToRemove.Add(listener);
		else _ = listeners.Remove(listener);
	}

	[UsedImplicitly]
	public void Trigger() {
		lock (syncObject) {
			triggering = true;

			foreach (var listener in listeners)
				listener.Trigger();

			if (readyToRemove.Count <= 0) return;
			foreach (var listener in readyToRemove)
				listeners.Remove(listener);

			triggering = false;
		}
	}
}

public class RefEvent<T> : ScriptableObject {
	private readonly HashSet<RefEventListener<T>> listeners = new HashSet<RefEventListener<T>>();
	private readonly List<RefEventListener<T>> readyToRemove = new List<RefEventListener<T>>();
	private readonly object syncObject = new object();
	private bool triggering;

	public void Add(RefEventListener<T> listener) {
		_ = listeners.Add(listener);
	}

	public void Remove(RefEventListener<T> listener) {
		if (triggering) readyToRemove.Add(listener);
		else _ = listeners.Remove(listener);
	}

	public void Trigger(T t) {
		lock (syncObject) {
			triggering = true;

			foreach (var listener in listeners)
				listener.Trigger(t);

			if (readyToRemove.Count <= 0) return;
			foreach (var listener in readyToRemove)
				listeners.Remove(listener);

			triggering = false;
		}
	}
}