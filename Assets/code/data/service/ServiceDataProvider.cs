using System.Collections.Generic;
using UnityEngine;

namespace data.service {
public class ServiceDataProvider : MonoBehaviour {
	// TODO: Not properly being deleted on close.
	private const string GameObject_Name = "service-data-provider";

	private static ServiceDataProvider dataProvider;
	private static bool quitting;

	private readonly Dictionary<string, MonoBehaviour> dataComponents = new Dictionary<string, MonoBehaviour>();

	private void Awake() {
		DontDestroyOnLoad(this);
	}

	private void OnApplicationQuit() {
		quitting = true;
	}

	private static ServiceDataProvider DataProvider {
		get {
			if (!Application.isPlaying || quitting)
				return null;
			if (dataProvider != null)
				return dataProvider;
			dataProvider = BuildProviderObject();
			return dataProvider;
		}
	}

	private static ServiceDataProvider BuildProviderObject() {
		var providerObject = new GameObject(GameObject_Name, typeof(ServiceDataProvider));
		return providerObject.GetComponent<ServiceDataProvider>();
	}

	/// <summary>
	/// Retrieve an existing Service data component by its type and Handle ID.
	/// Handle IDs are unique
	/// </summary>
	/// <param name="handleId"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static T Get<T>(string handleId) where T : MonoBehaviour {
		if (DataProvider == null) return null;
		var components = DataProvider.dataComponents;
		if (!components.ContainsKey(handleId)) return null;
		var dataComponent = components[handleId];
		return dataComponent as T;
	}

	/// <summary>
	/// Gets the first data component on the Provider game object which match the
	/// specified type.
	/// </summary>
	/// <typeparam name="T">The Component Type to search for.</typeparam>
	/// <returns>A reference to the Component.</returns>
	public static T GetFirstOfType<T>() where T : class
		=> DataProvider == null ? null : DataProvider.GetComponent<T>();

	/// <summary>
	/// Gets all data components attached to the provider which match the
	/// specified Type.
	/// </summary>
	/// <typeparam name="T">The Component Type to search for.</typeparam>
	/// <returns>An array of all matching Components.</returns>
	public static T[] GetAllOfType<T>() where T : class 
		=> DataProvider == null ? new T[0] : dataProvider.GetComponents<T>();

	/// <summary>
	/// Creates and attaches the given component type onto the service provider
	/// game object and returns a reference. If a component has already been
	/// published with the given handle, it will destroy and replace the old
	/// component.
	/// </summary>
	/// <param name="handleId">The identifying name of this data component instance.</param>
	/// <typeparam name="T">The Type of the Component to create.</typeparam>
	/// <returns>A reference to the final component.</returns>
	public static T Set<T>(string handleId) where T : MonoBehaviour {
		if (DataProvider == null) return null;
		var oldComponent = Get<T>(handleId);
		if (oldComponent != null) Destroy(oldComponent);
		var newComponent = DataProvider.gameObject.AddComponent<T>();
		DataProvider.dataComponents[handleId] = newComponent;
		return newComponent;
	}
}
}