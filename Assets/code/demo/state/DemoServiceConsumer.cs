using UnityEngine;

namespace demo.state {
public class DemoServiceConsumer : MonoBehaviour {
	public ServiceA serviceA;
	public ServiceB serviceB;

	private void Start() {
		Debug.LogFormat("ServiceA value = {0}", serviceA.Value);
		Debug.LogFormat("ServiceB value = {0}", serviceB.Value);
	}
}
}