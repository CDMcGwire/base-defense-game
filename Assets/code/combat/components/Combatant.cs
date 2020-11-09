using UnityEngine;

namespace combat.components {
/// <summary>
/// Component that defines a <see cref="GameObject"/> as a valid combat actor.
/// </summary>
public class Combatant : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private long pointsPerKill;
	[SerializeField] private long moneyPerKill;
#pragma warning restore 0649

	public long PointsPerKill => pointsPerKill;

	public long MoneyPerKill => moneyPerKill;
}
}