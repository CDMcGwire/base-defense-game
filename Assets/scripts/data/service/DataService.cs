using UnityEngine;

namespace data.service {
public abstract class DataService : ScriptableObject {
	protected virtual string HandleId => name;
	public abstract void Initialize();
	public void Erase() => ServiceDataProvider.Remove(HandleId);
}

public abstract class DataService<T> : DataService where T : MonoBehaviour {
	private T data;

	protected T Data => data ? data : RetrieveData();

	private T RetrieveData() {
		data = ServiceDataProvider.Get<T>(HandleId);
		if (data != null) return data;
		data = ServiceDataProvider.Set<T>(HandleId);
		if (data == null) return null;
		Initialize();
		return data;
	}
}
}