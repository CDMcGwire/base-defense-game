using data.service;
using UnityEngine;

namespace demo.state {
[CreateAssetMenu(fileName = "service-a", menuName = "Demo/ServiceA", order = 0)]
public class ServiceA : Service<DataHolderA> {
	public string Value => DataComponent != null ? DataComponent.value : "";
}
}