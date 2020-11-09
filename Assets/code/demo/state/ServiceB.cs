using data.service;
using UnityEngine;

namespace demo.state {
[CreateAssetMenu(fileName = "service-b", menuName = "Demo/ServiceB", order = 0)]
public class ServiceB : Service<DataHolderB> {
	public int Value => DataComponent != null ? DataComponent.value : 0;
}
}