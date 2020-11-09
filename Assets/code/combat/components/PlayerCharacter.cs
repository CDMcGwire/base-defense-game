using System;
using System.Collections.Generic;
using combat.weapon;
using data.game;
using JetBrains.Annotations;
using player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace combat.components {
public class PlayerCharacter : Combatant {
#pragma warning disable 0649
	[SerializeField] private Transform weaponModelSlot;
	[SerializeField] private PlayerInputAimer aimer;
#pragma warning restore 0649

	private PlayerLoadoutService loadout;
	private Weapon currentWeapon;

	private readonly Dictionary<string, Weapon> spawnedWeapons = new Dictionary<string, Weapon>();
	
	public void Initialize(PlayerLoadoutService playerLoadout, Camera playerCamera) {
		aimer = GetComponent<PlayerInputAimer>();
		aimer.RefCamera = playerCamera;

		var playerInput = GetComponent<PlayerInput>();
		playerInput.camera = playerCamera;

		if (playerLoadout == null)
			throw new ArgumentNullException($"PlayerCharacter {name} should not be initialized with a null loadout.");
		loadout = playerLoadout;
		if (loadout.EquippedWeapons.Count < 1) return;
		SwitchWeapon(loadout.ActiveWeaponSlot);
	}

	[UsedImplicitly]
	public void SwitchWeapon(int slot) {
		Debug.Log("Switching weapon to " + slot);
		if (slot < 0 || slot >= loadout.EquippedWeapons.Count)
			return;
		var weaponPrefab = loadout.EquippedWeapons[slot]?.GetDataAs<Weapon>();

		if (currentWeapon != null)
			currentWeapon.gameObject.SetActive(false);
		if (weaponPrefab == null) {
			currentWeapon = null;
			aimer.Targeter = weaponModelSlot;
			return;
		}

		if (!spawnedWeapons.ContainsKey(weaponPrefab.name))
			spawnedWeapons[weaponPrefab.name] = SpawnWeapon(weaponPrefab);
		var targetWeapon = spawnedWeapons[weaponPrefab.name];

		targetWeapon.gameObject.SetActive(true);
		currentWeapon = targetWeapon;
		loadout.ActiveWeaponSlot = slot;
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
		var target = (loadout.ActiveWeaponSlot + direction) % loadout.EquippedWeapons.Count;
		SwitchWeapon(target);
	}

	[UsedImplicitly]
	public void FireActiveWeapon(InputAction.CallbackContext inputContext) {
		if (currentWeapon == null) return;
		currentWeapon.HandleInput(inputContext);
	}

	private Weapon SpawnWeapon(Weapon weaponPrefab) {
		var spawnedWeapon = Instantiate(weaponPrefab, weaponModelSlot);
		spawnedWeapon.Owner = this;
		return spawnedWeapon;
	}
}
}