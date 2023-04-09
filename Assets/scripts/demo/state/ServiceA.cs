using data.service;
using UnityEngine;

namespace demo.state {
[CreateAssetMenu(fileName = "service-a", menuName = "Demo/ServiceA", order = 0)]
public class ServiceA : DataService<DataHolderA> {
	public string Value => Data != null ? Data.value : "";
	public override void Initialize() { }
}
}