using data.service;
using UnityEngine;

namespace demo.state {
[CreateAssetMenu(fileName = "service-b", menuName = "Demo/ServiceB", order = 0)]
public class ServiceB : DataService<DataHolderB> {
	public int Value => Data != null ? Data.value : 0;
	public override void Initialize() { }
}
}