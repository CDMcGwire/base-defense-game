using data.reactive;
using data.service;
using UnityEngine;

namespace data.refvalues {
public abstract class RefValue : DataService<RefValueStorage> {
	public abstract string AsText();
	public abstract void Coerce(string text);
}

public abstract class RefValue<T> : RefValue {
#pragma warning disable 0649
	[SerializeField] private T initialValue;
#pragma warning restore 0649

	protected override string HandleId => "ref-values";

	public RxVal<T> Value
		=> Data != null
			? Data.Lookup<T>(name)
			  ?? Data.Publish(name, initialValue)
			: null;

	public override void Initialize()
		=> Value.Current = initialValue;

	public override string AsText()
		=> Value.Current.ToString();
}
}