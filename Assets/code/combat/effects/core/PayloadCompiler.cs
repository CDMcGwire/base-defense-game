using System.Collections.Generic;

namespace combat.effects.core {
public static class PayloadCompiler {
	// ReSharper disable Unity.PerformanceAnalysis : It's not actually called more than once per object at most.
	/// <summary>
	///   Applies the rules for a payload chain to an enumerable collection of payload
	///   objects and stores the results in the supplied list. Storing in the list is
	///   improve memory allocation efficiency by allowing the caller to reuse a
	///   pre-allocated object. The given list is cleared eagerly.
	/// </summary>
	/// <param name="payloadObjects">The objects to compile into a chain.</param>
	/// <param name="chainContainer">The list that should hold the final chain.</param>
	/// <typeparam name="T">
	///   The sub-type of PayloadObject that the chain will contain.
	/// </typeparam>
	public static void Build<T>(IEnumerable<T> payloadObjects, List<T> chainContainer) where T : PayloadObject<T> {
		chainContainer.Clear();
		// Group
		var groupings = new Dictionary<string, List<T>>();
		foreach (var obj in payloadObjects) {
			if (!groupings.ContainsKey(obj.Group))
				groupings[obj.Group] = new List<T>();
			groupings[obj.Group].Add(obj);
		}

		foreach (var group in groupings.Values) {
			// Filter
			// Get the highest precedence
			var highestPrecedence = int.MinValue;
			foreach (var effect in group)
				if (effect.Precedence > highestPrecedence)
					highestPrecedence = effect.Precedence;

			var filteredList = new List<T>();
			foreach (var effect in group)
				if (effect.Precedence >= highestPrecedence)
					filteredList.Add(effect);

			// Order - PayloadObject<T> implements IComparable<T>
			filteredList.Sort();

			// Stack
			var stackedList = new List<T>();
			var stackedEffect = filteredList[0];
			var stackCount = 1;
			for (var i = 1; i < filteredList.Count; i++)
				if (filteredList[i].GetType() == stackedEffect.GetType()) {
					stackedEffect = stackedEffect.Stack(filteredList[i], ++stackCount);
				}
				else {
					stackedList.Add(stackedEffect);
					stackedEffect = filteredList[i];
					stackCount = 1;
				}
			stackedList.Add(stackedEffect); // Put last effect onto the list
			chainContainer.AddRange(stackedList);
		}
	}

	/// <summary>
	///   Shorthand method to compile a collection of Mods sorted by kind.
	/// </summary>
	/// <param name="modsByKind">The pre-sorted mods.</param>
	/// <param name="modsContainer">
	///   The dictionary that should hold the final chains according to their kind.
	/// </param>
	public static void BuildModChains(
		IReadOnlyDictionary<string, List<CombatMod>> modsByKind,
		ModsByKind modsContainer
	) {
		modsContainer.Clear();

		foreach (var entry in modsByKind) {
			var chain = new List<CombatMod>();
			Build(entry.Value, chain);
			modsContainer[entry.Key] = chain;
		}
	}
}

public class ModsByKind : Dictionary<string, IReadOnlyList<CombatMod>> {
	public IReadOnlyList<CombatMod> Global => OfKind(CombatMod.Global);

	public IReadOnlyList<CombatMod> OfKind(string kind) {
		return ContainsKey(kind)
			? this[kind]
			: new List<CombatMod>(0);
	}
}
}