using UnityEngine;

namespace data.condition {
/// <summary>
/// Scriptable object instances that are designed to read other scriptable
/// objects to determine if some in-game condition is true.
/// </summary>
public abstract class DataCondition : ScriptableObject {
	public abstract bool Evaluate();
}
}