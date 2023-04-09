using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace data.inventory {
[CreateAssetMenu(fileName = "new-item", menuName = "Item")]
public class InventoryItem : ScriptableObject, IEquatable<InventoryItem> {
#pragma warning disable 0649
	[SerializeField] private string displayName;
	[SerializeField] private Sprite sprite;
	[SerializeField] private int price;
	[SerializeField] private bool unique;
	[Space(10)]
	[SerializeField] private Object data;
#pragma warning restore 0649
	
	private HashSet<Type> dataTypes = new();

	public string DisplayName => displayName;
	public Sprite Sprite => sprite;
	public int Price => price;
	public bool Unique => unique;
	public Object Data => data;

	public bool HasDataType<T>() {
		if (dataTypes.Count >= 1) return dataTypes.Contains(typeof(T));
		if (Data is GameObject go) {
			foreach (var component in go.GetComponents<MonoBehaviour>())
				dataTypes.Add(component.GetType());
		}
		else dataTypes.Add(Data.GetType());
		return dataTypes.Contains(typeof(T));
	}

	public T GetDataAs<T>() where T: class {
		if (!HasDataType<T>()) return null;
		switch (Data) {
			case T typedData: return typedData;
			case GameObject gameObject: return gameObject.GetComponent<T>();
			default: return null;
		}
	}

	public bool Equals(InventoryItem other)
		=> other != null && name == other.name;

	public override bool Equals(object obj) {
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		return obj.GetType() == GetType()
		       && Equals((InventoryItem) obj);
	}

	public override int GetHashCode()
		=> base.GetHashCode();

	public static bool operator ==(InventoryItem left, InventoryItem right)
		=> Equals(left, right);

	public static bool operator !=(InventoryItem left, InventoryItem right)
		=> !Equals(left, right);
}
}