namespace utility.extensions {
public static class ArrayExtensions {
	/// <summary>
	/// Creates a new array of the specified size, copying over as much as the
	/// source array's contents as will fit. If the source array is larger than
	/// the new size, the leftover contents will be discarded. If the source
	/// array is smaller than the new size, the remaining entries will be the
	/// type's default.
	/// </summary>
	/// <param name="original">The source array.</param>
	/// <param name="size">The desired size of the new array.</param>
	/// <typeparam name="T">The type of the array contents.</typeparam>
	/// <returns>A newly allocated array of the given size.</returns>
	public static T[] CopyToSize<T>(this T[] original, int size) {
		var sizedArray = new T[size];
		var minLength = size <= original.Length ? size : original.Length;
		for (var i = 0; i < minLength; i++)
			sizedArray[i] = original[i];
		return sizedArray;
	}
}
}