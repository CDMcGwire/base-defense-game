using System;
using UnityEngine;

namespace data.loadout {
[CreateAssetMenu(fileName = "new-combat-tool", menuName = "Loadout/Tool", order = 0)]
public class ToolData : ScriptableObject, IEquatable<ToolData> {
	public bool Equals(ToolData other)
		=> other != null && name == other.name;

	public override bool Equals(object obj) {
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		return obj.GetType() == GetType()
		       && Equals((ToolData) obj);
	}

	public override int GetHashCode()
		=> base.GetHashCode();

	public static bool operator ==(ToolData left, ToolData right)
		=> Equals(left, right);

	public static bool operator !=(ToolData left, ToolData right)
		=> !Equals(left, right);
}
}