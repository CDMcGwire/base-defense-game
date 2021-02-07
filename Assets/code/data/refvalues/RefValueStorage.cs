using System.Collections.Generic;
using data.reactive;
using UnityEngine;

namespace data.refvalues {
public class RefValueStorage : MonoBehaviour {
	private readonly Dictionary<string, RxVal> values = new Dictionary<string, RxVal>();

	public RxVal<T> Lookup<T>(string key) {
		if (values.ContainsKey(key))
			return values[key] is RxVal<T> observable
				? observable
				: null;
		return null;
	}

	public RxVal<T> Publish<T>(string key, T value) {
		var observable = new RxVal<T>(value);
		values[key] = observable;
		return observable;
	}
}
}