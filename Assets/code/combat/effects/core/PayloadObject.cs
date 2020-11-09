using System;
using UnityEngine;

namespace combat.effects.core {
[Serializable]
public abstract class PayloadObject<T> : IComparable<PayloadObject<T>> where T : PayloadObject<T> {
#pragma warning disable 0649
	[SerializeField] private string group;
	[SerializeField] private string kind;
	[SerializeField] private int order;
	[SerializeField] private int precedence;
#pragma warning restore 0649

	/// <summary>
	///   Group determines what other PayloadObjects the instance will be compared to
	///   when filtering, stacking, and ordering. Only instances of like group will be
	///   checked.
	/// </summary>
	public string Group => group;

	/// <summary>
	///   Kind is used in payload resolution to determine what objects should modify
	///   each other. Only objects of like Kind will be matched.
	/// </summary>
	public string Kind => kind;

	/// <summary>
	///   Precedence is used to filter out objects in a Group by relative importance.
	///   Only objects with a precedence equal to the highest found in the group will
	///   be retained.
	/// </summary>
	public int Precedence => precedence;

	public int CompareTo(PayloadObject<T> other) {
		var result = order.CompareTo(other.order);
		return result != 0 ? result : string.Compare(GetType().Name, other.GetType().Name, StringComparison.Ordinal);
	}

	/// <summary>
	///   Required to be implemented by concrete types to determine how
	///   <see cref="PayloadObject{T}" /> of the same type should combine when applied
	///   at the same resolution order.
	/// </summary>
	/// <param name="next">The next payload object to stack.</param>
	/// <param name="stackCount">
	///   The total number of items stacked so far, including the one being stacked.
	/// </param>
	/// <returns>The resulting <see cref="PayloadObject{T}" /></returns>
	public abstract T Stack(T next, int stackCount);
}

public abstract class PayloadDataSource : ScriptableObject, IEquatable<PayloadDataSource> {
	public bool Equals(PayloadDataSource other)
		=> other != null && name == other.name;

	public override bool Equals(object other) {
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return other.GetType() == GetType()
		       && Equals((PayloadDataSource) other);
	}

	public override int GetHashCode()
		=> base.GetHashCode();

	public static bool operator ==(PayloadDataSource left, PayloadDataSource right)
		=> Equals(left, right);

	public static bool operator !=(PayloadDataSource left, PayloadDataSource right)
		=> !Equals(left, right);
}

public abstract class PayloadDataSource<T> : PayloadDataSource where T : PayloadObject<T> {
	public abstract T Value { get; }
}
}