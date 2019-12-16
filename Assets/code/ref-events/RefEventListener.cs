using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("Listener/SimpleListener", 1)]

public class RefEventListener : MonoBehaviour {
	[SerializeField]
	private RefEvent refEvent;
	public RefEvent RefEvent {
		get => refEvent;
		set => refEvent = value;
	}

	[SerializeField]
	private UnityEvent onTrigger;
	public UnityEvent OnTrigger => onTrigger;

	public void Trigger() => onTrigger.Invoke();

	public void OnEnable() => refEvent.Add(this);

	public void OnDisable() => refEvent.Remove(this);
}

public class RefEventListener<T> : MonoBehaviour {
	public RefEvent<T> refEvent;

	public UnityEvent<T> onTrigger;

	public void Trigger(T t) => onTrigger.Invoke(t);

	public void OnEnable() => refEvent.Add(this);

	public void OnDisable() => refEvent.Remove(this);
}
