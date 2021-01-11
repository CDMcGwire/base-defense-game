using System;
using data.game;
using UnityEngine;

namespace combat.components {
public class Castle : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private CastleManagementService castleManagementService;
	[SerializeField] private DamageComponent damageComponent;
#pragma warning restore 0649

	private Action<long, long> healthChangeAction;

	private void OnEnable() {
		damageComponent.MaxHealth = castleManagementService.Health.Current;
		healthChangeAction = (last, current) => damageComponent.MaxHealth = current;
		castleManagementService.Health.OnChanged += healthChangeAction;

		damageComponent.Damage = castleManagementService.Damage.Current;
		damageComponent.OnDamaged.AddListener(castleManagementService.RecordDamage);
	}

	private void OnDisable() {
		var healthVal = castleManagementService.Health;
		if (healthVal != null)
			healthVal.OnChanged -= healthChangeAction;
		damageComponent.OnDamaged.RemoveListener(castleManagementService.RecordDamage);
	}
}
}