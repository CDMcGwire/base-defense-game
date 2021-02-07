using System.Collections.Generic;

namespace utility.extensions {
public static class ListExtensions {
	public static bool IndexOutOfBounds<T>(this IList<T> list, int index)
		=> index < 0 || index >= list.Count;
}
}