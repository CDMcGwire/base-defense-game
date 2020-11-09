using UnityEngine;

namespace data.service {
public abstract class Service : ScriptableObject { }

public abstract class Service<T> : Service where T : MonoBehaviour {
	private T dataObject;

	protected virtual string HandleId => name;

	protected T DataComponent {
		get {
			if (Application.isPlaying && dataObject == null)
				RetrieveServiceData();
			return dataObject;
		}
	}

	protected virtual void InitializeDataObject(T data) { }

	private void RetrieveServiceData() {
		dataObject = ServiceDataProvider.Get<T>(HandleId);
		if (dataObject == null) {
			dataObject = ServiceDataProvider.Set<T>(HandleId);
			InitializeDataObject(dataObject);
		}
	}
}
}