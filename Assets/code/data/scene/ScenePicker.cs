using UnityEngine;

namespace data.scene {
/// <summary>
/// Object which provides the name of the next scene to load, as needed by
/// Unity's Scene Management interface.
/// </summary>
public abstract class ScenePicker : ScriptableObject {
	public abstract string Next();
}
}