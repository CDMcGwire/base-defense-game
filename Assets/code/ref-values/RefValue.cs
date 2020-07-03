using UnityEngine;

public class RefValue<T> : ScriptableObject {
	private T current;

	private bool dirty;
	[SerializeField]
	private T initial;
	public T Initial => initial;

	public T Current {
		get => dirty ? current : initial;
		set {
			dirty = true;
			current = value;
		}
	}

	public void OnEnable() {
		Current = initial;
	}

	public void Save() {
		initial = Current;
		dirty = false;
	}

	public void Save(T t) {
		initial = Current = t;
		dirty = false;
	}
}