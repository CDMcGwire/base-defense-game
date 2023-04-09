using System;
using UnityEngine;

namespace combat.castle {
[CreateAssetMenu(fileName = "turret-data", menuName = "Combat/Turret", order = 0)]
public class TurretData : ScriptableObject, IEquatable<TurretData> {
#pragma warning disable 0649
	[SerializeField] private string type;
	[SerializeField] private Turret turretPrefab;
	[SerializeField] private Sprite sprite;
	[TextArea(2, 4)]
	[SerializeField] private string description;
#pragma warning restore 0649

	public string Type => type;
	public Turret TurretPrefab => turretPrefab;
	public Sprite Sprite => sprite;
	public string Description => description;

	public bool Equals(TurretData other) {
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return base.Equals(other)
		       && other.GetInstanceID() == GetInstanceID()
		       && string.Equals(type, other.type, StringComparison.OrdinalIgnoreCase);
	}

	public override bool Equals(object obj) {
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		return obj.GetType() == GetType() && Equals((TurretData) obj);
	}

	public override int GetHashCode() {
		unchecked {
			return (base.GetHashCode() * 397) ^ (type != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(type) : 0);
		}
	}

	public static bool operator ==(TurretData left, TurretData right) => Equals(left, right);

	public static bool operator !=(TurretData left, TurretData right) => !Equals(left, right);
}
}