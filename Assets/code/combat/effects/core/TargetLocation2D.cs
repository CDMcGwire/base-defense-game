using UnityEngine;

namespace combat.effects.core {
public readonly struct TargetLocation2D {
	/// <summary>
	/// The collider hit by the targeting logic.
	/// </summary>
	public readonly Collider2D collider;

	/// <summary>
	/// The attached rigidbody hit by targeting logic, if present.
	/// </summary>
	public readonly Rigidbody2D rigidbody;

	/// <summary>
	/// The world space coordinates of the point on the collider that was targeted.
	/// </summary>
	public readonly Vector2 point;

	/// <summary>
	/// The surface normal of the target point on the collider.
	/// </summary>
	public readonly Vector2 normal;

	/// <summary>
	/// The location the targeting check originated from.
	/// </summary>
	public readonly Vector2 origin;

	/// <summary>
	/// The distance between the targeted point and the origin.
	/// </summary>
	public readonly float distance;

	/// <summary>
	/// The Unity Instance ID that should be associated with the target. From the rigidbody if present, else just the
	/// collider's.
	/// </summary>
	public readonly int id;

	public TargetLocation2D(
		Collider2D collider,
		Vector2 point,
		Vector2 normal,
		Vector2 origin,
		Rigidbody2D rigidbody = default
	) : this() {
		this.collider = collider;
		this.point = point;
		this.normal = normal;
		this.origin = origin;
		this.rigidbody = rigidbody;
		
		distance = Vector2.Distance(origin, point);
		id = rigidbody != null ? rigidbody.GetInstanceID() : collider.GetInstanceID();
	}

	public TargetLocation2D(RaycastHit2D hit)
		: this(
			hit.collider,
			hit.point,
			hit.normal,
			hit.centroid,
			hit.rigidbody
		) { }
}
}