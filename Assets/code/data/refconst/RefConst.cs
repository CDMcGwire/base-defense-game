using UnityEngine;

namespace data.refconst {
public abstract class RefConst : ScriptableObject { }

public abstract class RefConst<T> : RefConst {
#pragma warning disable 0649
	[SerializeField] private T value;
#pragma warning restore 0649

	public T Value => value;
}
}