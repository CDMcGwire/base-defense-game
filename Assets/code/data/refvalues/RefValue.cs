using System;
using data.service;
using UnityEngine;

namespace data.refvalues {
public abstract class RefValue : Service<RefValueStorage> {
	public abstract void Reset();
	public abstract string ToJson();
	public abstract void FromJson(string json);
}

public abstract class RefValue<T> : RefValue {
#pragma warning disable 0649
	[SerializeField] private T initialValue;
#pragma warning restore 0649

	protected override string HandleId => "ref-values";

	public Observable<T> Value 
		=> DataComponent != null 
			? DataComponent.Lookup<T>(name) ?? DataComponent.Publish(name, initialValue) 
			: null;

	public override void Reset() => Value.Current = initialValue;

	public override string ToJson()
		=> JsonUtility.ToJson(new ValueWrapper {value = Value.Current});

	public override void FromJson(string json)
		=> Value.Current = JsonUtility.FromJson<ValueWrapper>(json).value;

	[Serializable]
	private struct ValueWrapper {
		public T value;
	}
}
}