using System;
using UnityEngine;

namespace combat.effects.core {
[Serializable]
public abstract class CombatMod : PayloadObject<CombatMod> {
	public static readonly string Global = "Global";
}

public abstract class CombatModData<T> : PayloadDataSource<CombatMod> where T : CombatMod {
#pragma warning disable 0649
	[SerializeField] private T effect;
#pragma warning restore 0649
	public override CombatMod Value => effect;
}
}