using System;
using System.Collections.Generic;
using combat.weapon;
using data.game;
using data.inventory;
using JetBrains.Annotations;
using player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace combat.components.update {
public class PlayerCharacter : Combatant {
#pragma warning disable 0649
	[SerializeField] private Transform weaponModelSlot;
	[SerializeField] private PlayerInputAimer aimer;
#pragma warning restore 0649

	private PlayerLoadoutService loadout;
	private Weapon currentWeapon;

	private readonly Weapon[] spawnedWeapons = Array.Empty<Weapon>();

	public void Initialize(PlayerLoadoutService playerLoadout, Camera playerCamera) {
		aimer = GetComponent<PlayerInputAimer>();
		aimer.RefCamera = playerCamera;

		var playerInput = GetComponent<PlayerInput>();
		playerInput.camera = playerCamera;

		if (playerLoadout == null)
			throw new ArgumentNullException($"PlayerCharacter {name} should not be initialized with a null loadout.");
		loadout = playerLoadout;
		if (loadout.EquippedWeapons.Count < 1) return;

		SpawnWeapons(loadout.EquippedWeapons, loadout.WeaponUpgradesByGun());

		SwitchWeapon(loadout.ActiveWeaponSlot.Current);
	}

	[UsedImplicitly]
	public void SwitchWeapon(int slot) {
		if (slot < 0 || slot >= spawnedWeapons.Length)
			return;
		if (spawnedWeapons[slot] == null)
			return;

		var targetWeapon = spawnedWeapons[slot];
		if (currentWeapon != null)
			currentWeapon.gameObject.SetActive(false);

		targetWeapon.gameObject.SetActive(true);
		currentWeapon = targetWeapon;
		loadout.ChangeActiveWeaponSlot(slot);
		aimer.Targeter = targetWeapon.MuzzlePoint;
	}

	[UsedImplicitly]
	public void SwitchWeapon01(InputAction.CallbackContext inputContext) {
		if (!inputContext.performed) return;
		SwitchWeapon(0);
	}

	[UsedImplicitly]
	public void SwitchWeapon02(InputAction.CallbackContext inputContext) {
		if (!inputContext.performed) return;
		SwitchWeapon(1);
	}

	[UsedImplicitly]
	public void SwitchWeapon03(InputAction.CallbackContext inputContext) {
		if (!inputContext.performed) return;
		SwitchWeapon(2);
	}

	[UsedImplicitly]
	public void SwitchWeapon04(InputAction.CallbackContext inputContext) {
		if (!inputContext.performed) return;
		SwitchWeapon(3);
	}

	[UsedImplicitly]
	public void CycleWeapon(int direction) {
		if (loadout.EquippedWeapons.Count < 1) return;
		var target = (loadout.ActiveWeaponSlot.Current + direction) % loadout.EquippedWeapons.Count;
		SwitchWeapon(target);
	}

	[UsedImplicitly]
	public void PullTrigger(InputAction.CallbackContext inputContext) {
		if (currentWeapon == null || !inputContext.performed) return;
		currentWeapon.BeginAttack();
	}

	[UsedImplicitly]
	public void ReleaseTrigger(InputAction.CallbackContext inputContext) {
		if (currentWeapon == null || !inputContext.performed) return;
		currentWeapon.ReleaseAttack();
	}

	[UsedImplicitly]
	public void FireActiveWeapon(InputAction.CallbackContext inputContext) {
		if (currentWeapon == null) return;
		currentWeapon.HandleInput(inputContext);
	}

	private void SpawnWeapons(
		IReadOnlyList<InventoryItem> equippedWeapons,
		IReadOnlyDictionary<string, IReadOnlyList<WeaponUpgrade>> weaponUpgradesByGun
	) {
		for (var i = 0; i < equippedWeapons.Count; i++) {
			var weaponEntry = equippedWeapons[i];
			if (weaponEntry == null)
				continue;
			var spawnedWeapon = Instantiate(weaponEntry.GetDataAs<Weapon>(), weaponModelSlot);
			spawnedWeapon.Owner = this;
			spawnedWeapon.Initialize(weaponUpgradesByGun[weaponEntry.name]);
			spawnedWeapons[i] = spawnedWeapon;
		}
	}

}
}