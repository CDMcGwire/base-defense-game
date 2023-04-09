using System;
using System.Collections.Generic;
using data.game;
using data.reactive;
using TMPro;
using ui.input;
using UnityEngine;

namespace ui.town {
public class TurretPlacementDiagram : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private List<Slot> turretEquipSlots;
#pragma warning restore 0649

	private readonly SortedList<string, TMP_Dropdown.OptionData> sortedTurretOptions = new();

	public event Action<int, string> OnSlotChanged;

	private void Start() {
		for (var i = 0; i < turretEquipSlots.Count; i++) {
			var slotIndex = i;

			void ValueChangedAction(int optionIndex) {
				var option = optionIndex < 0
					? null
					: sortedTurretOptions.Values[optionIndex].text;
				OnSlotChanged?.Invoke(slotIndex, option);
			}

			turretEquipSlots[i].OnValueChanged.AddListener(ValueChangedAction);
		}
	}

	public void Wire(
		IRxReadonlyList<OwnedTurret> ownedTurrets,
		IRxReadonlyList<OwnedTurret> equippedTurrets
	) {
		ownedTurrets.Bind(
			OwnedTurretsRestructureAction,
			OwnedTurretsAddAction,
			OwnedTurretsRemoveAction,
			OwnedTurretsChangeAction
		);
		equippedTurrets.Bind(
			EquippedTurretsRestructureAction,
			EquippedTurretsAddAction,
			EquippedTurretsRemoveAction,
			EquippedTurretsChangeAction
		);
	}

	private void OwnedTurretsRestructureAction(IReadOnlyList<OwnedTurret> turrets) {
		sortedTurretOptions.Clear();
		foreach (var turret in turrets)
			sortedTurretOptions.Add(
				turret.Data.Type,
				new TMP_Dropdown.OptionData(turret.Data.Type, turret.Data.Sprite)
			);
		foreach (var slot in turretEquipSlots)
			slot.Populate(sortedTurretOptions.Values);
	}

	private void OwnedTurretsAddAction(OwnedTurret turret, int index) {
		var option = new TMP_Dropdown.OptionData(turret.Data.Type, turret.Data.Sprite);
		sortedTurretOptions.Add(turret.Data.Type, option);
		var finalIndex = sortedTurretOptions.IndexOfKey(turret.Data.Type);
		foreach (var slot in turretEquipSlots)
			slot.Insert(finalIndex, option);
	}

	private void OwnedTurretsRemoveAction(OwnedTurret turret, int index) {
		var type = turret.Data.Type;
		var finalIndex = sortedTurretOptions.IndexOfKey(type);
		sortedTurretOptions.Remove(type);
		foreach (var slot in turretEquipSlots)
			slot.RemoveAt(finalIndex);
	}

	private void OwnedTurretsChangeAction(OwnedTurret previous, OwnedTurret turret, int index) {
		var previousType = previous.Data.Type;
		var type = turret.Data.Type;
		var option = new TMP_Dropdown.OptionData(type, turret.Data.Sprite);
		if (previousType == type) {
			sortedTurretOptions[type] = option;
			foreach (var slot in turretEquipSlots)
				slot.Set(sortedTurretOptions.IndexOfKey(type), option);
		}
		else {
			var previousIndex = sortedTurretOptions.IndexOfKey(previousType);
			var finalIndex = sortedTurretOptions.IndexOfKey(type);
			sortedTurretOptions.Remove(previousType);
			sortedTurretOptions.Add(type, option);
			foreach (var slot in turretEquipSlots)
				slot.RemoveAt(previousIndex);
			foreach (var slot in turretEquipSlots)
				slot.Insert(finalIndex, option);
		}
	}

	private void EquippedTurretsRestructureAction(IReadOnlyList<OwnedTurret> turrets) {
		var minUpdatable = Mathf.Min(turrets.Count, turretEquipSlots.Count);
		for (var i = 0; i < minUpdatable; i++) {
			var data = turrets[i].Data;
			var slot = turretEquipSlots[i];
			if (sortedTurretOptions.ContainsKey(data.Type))
				slot.Value = sortedTurretOptions.IndexOfKey(data.Type);
			else {
				slot.Value = -1;
				slot.SetShown(data.Type, data.Sprite);
			}
		}
	}

	private void EquippedTurretsAddAction(OwnedTurret turret, int index) {
		if (turret == null) return;
		if (index >= turretEquipSlots.Count) return;

		var slot = turretEquipSlots[index];
		if (sortedTurretOptions.ContainsKey(turret.Data.Type))
			slot.Value = sortedTurretOptions.IndexOfKey(turret.Data.Type);
		else {
			slot.Value = -1;
			slot.SetShown(turret.Data.Type, turret.Data.Sprite);
		}
	}

	private void EquippedTurretsRemoveAction(OwnedTurret turret, int index) {
		if (turret == null) return;
		if (index < turretEquipSlots.Count)
			turretEquipSlots[index].Value = -1;
	}

	private void EquippedTurretsChangeAction(OwnedTurret previous, OwnedTurret turret, int index) {
		if (index >= turretEquipSlots.Count)
			return;
		var slot = turretEquipSlots[index];
		if (turret == null)
			slot.Value = -1;
		else if (sortedTurretOptions.ContainsKey(turret.Data.Type))
			slot.Value = sortedTurretOptions.IndexOfKey(turret.Data.Type);
		else {
			slot.Value = -1;
			slot.SetShown(turret.Data.Type, turret.Data.Sprite);
		}
	}
}
}