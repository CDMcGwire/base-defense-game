﻿using UnityEngine;
using UnityEngine.Events;

namespace data.refevents {
[AddComponentMenu("Listener/SimpleListener", 1)]
public class RefEventListener : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private RefEvent refEvent;
	[SerializeField] private UnityEvent onTrigger;
#pragma warning restore 0649
	
	public RefEvent RefEvent {
		get => refEvent;
		set => refEvent = value;
	}

	public UnityEvent OnTrigger => onTrigger;

	public void Trigger() {
		onTrigger.Invoke();
	}

	public void OnEnable() {
		refEvent.Add(this);
	}

	public void OnDisable() {
		refEvent.Remove(this);
	}
}

public class RefEventListener<T> : MonoBehaviour {
	public RefEvent<T> refEvent;
	public UnityEvent<T> onTrigger;

	public void Trigger(T t) {
		onTrigger.Invoke(t);
	}

	public void OnEnable() {
		refEvent.Add(this);
	}

	public void OnDisable() {
		refEvent.Remove(this);
	}
}
}