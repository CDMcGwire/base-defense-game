using System.Collections.Generic;

namespace combat.targeting {
public static class TargetFilter { // TODO: Maybe make this its own component so a filter list can be shared. Or maybe a ScriptableObject? Actually, might still want include/exclude logic at some point.
	public static void Build(IEnumerable<string> targetTypes, HashSet<int> filter) {
		filter.Clear();
		foreach (var type in targetTypes) 
			filter.Add(type.ToLower().Trim().GetHashCode());
	}
}
}