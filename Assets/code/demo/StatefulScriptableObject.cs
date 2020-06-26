using UnityEngine;

namespace demo {
[CreateAssetMenu(fileName = "stateful-object", menuName = "Demo/Stateful SO", order = 0)]
public class StatefulScriptableObject : ScriptableObject {
	public string value = "";

	public void AddStuff(string stuff) => value += stuff;
}
}