using data.refconst;
using UnityEngine;

namespace data.scene {
/// <summary>
/// Loads a statically configured scene.
/// </summary>
[CreateAssetMenu(fileName = "simple-scene-picker", menuName = "Scene Picker/Simple", order = 0)]
public class SimpleScenePicker : ScenePicker {
#pragma warning disable 0649
	[SerializeField] private RefConst<string> scene;
#pragma warning restore 0649

	public override string Next() => scene.Value;
}
}