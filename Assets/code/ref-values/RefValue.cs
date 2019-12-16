using UnityEngine;

public class RefValue<T> : ScriptableObject {
	[SerializeField]
	private T initial;
	public T Initial => initial;

	private bool dirty = false;

	private T current;
	public T Current {
		get => dirty ? current : initial;
		set {
			dirty = true;
			current = value;
		}
	}

	public void OnEnable() => Current = initial;

	public void Save() {
		initial = Current;
		dirty = false;
	}

	public void Save(T t) {
		initial = Current = t;
		dirty = false;
	}
}
