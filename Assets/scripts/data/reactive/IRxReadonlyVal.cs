using System;

namespace data.reactive {
/// <summary>
/// An interface to handle a RxVal with for read-only access.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRxReadonlyVal<out T> {
	/// <summary>
	/// The value stored in the object at this moment in time.
	/// </summary>
	T Current { get; }

	/// <summary>
	/// An event raised whenever the value of Current changes. The first
	/// parameter is the previous value, the second is the new value.
	/// </summary>
	event Action<T> OnChanged;

	/// <summary>
	/// Add an event handler to OnChanged and immediately invoke it with the
	/// current value.
	/// </summary>
	/// <param name="action">The event handler to bind.</param>
	void Bind(Action<T> action);
}
}