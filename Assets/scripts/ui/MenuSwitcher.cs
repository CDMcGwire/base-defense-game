using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace ui {
public class MenuSwitcher : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private string rootMenuName = "";
	[SerializeField] private bool rootOnDisable = true;
	[SerializeField] private bool simultaneousOpenClose = false;
	[SerializeField] private GameMenuGroupEntry[] menus;
#pragma warning restore 0649

	private readonly LinkedList<string> history = new();
	private readonly Dictionary<string, GameMenu> menuLookup = new();
	private string currentMenuName = "";

#if UNITY_EDITOR
	private void OnValidate() {
		rootMenuName = rootMenuName.Trim();
		if (Application.IsPlaying(this) && menuLookup.Count > 0) CompileMenuLookup();
	}
#endif

	private void OnEnable() {
		CompileMenuLookup();
		var shouldInitialize = currentMenuName.Length < 1 && rootMenuName.Length > 0;
		if (shouldInitialize) Change(rootMenuName);
	}

	private void OnDisable() {
		if (rootOnDisable) Root(true);
	}

	[UsedImplicitly]
	public void Change(string menuName) {
		// Trim and validate the target menu value.
		var trimmedMenuName = menuName.Trim();
		if (currentMenuName == trimmedMenuName) {
			// Edge case where the current menu was closed manually.
			var currentMenu = menuLookup[trimmedMenuName];
			if (!currentMenu.gameObject.activeInHierarchy)
				currentMenu.Open();
			return;
		}
		if (!menuLookup.ContainsKey(trimmedMenuName)) {
			Debug.LogWarning($"Tried to change to menu {trimmedMenuName} from group {name} but it was not found");
			return;
		}

		if (currentMenuName.Length < 1) {
			menuLookup[trimmedMenuName].Open();
		}
		else {
			var currentMenu = menuLookup[currentMenuName];
			var nextMenu = menuLookup[trimmedMenuName];
			SwitchMenus(currentMenu, nextMenu);
		}
		_ = history.Remove(trimmedMenuName);
		history.AddLast(currentMenuName);
		currentMenuName = trimmedMenuName;
	}

	[UsedImplicitly]
	public void Return() {
		if (history.Count < 1) return;

		var currentMenu = menuLookup[currentMenuName];
		currentMenuName = history.Last.Value;

		if (currentMenuName.Length < 1) {
			currentMenu.Close();
		}
		else {
			var lastMenu = menuLookup[currentMenuName];
			SwitchMenus(currentMenu, lastMenu);
		}
		history.RemoveLast();
	}

	[UsedImplicitly]
	public void Root(bool immediate) {
		if (history.Count < 1) return;

		var currentMenu = menuLookup[currentMenuName];
		currentMenuName = rootMenuName;

		if (rootMenuName.Length < 1) {
			currentMenu.Close(immediate);
		}
		else {
			var initialMenu = menuLookup[rootMenuName];
			currentMenu.Close(initialMenu.Open, immediate);
		}
		history.Clear();
	}

	[UsedImplicitly]
	public void Root() => Root(false);

	private void CompileMenuLookup() {
		menuLookup.Clear();
		foreach (var entry in menus) {
			var trimmedName = entry.name.Trim();
			menuLookup[trimmedName] = entry.menu;
			if (trimmedName != currentMenuName)
				entry.menu.gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Performs the swap operation if changing from one menu to another.
	/// </summary>
	/// <param name="current">The menu that is currently open.</param>
	/// <param name="next">The menu that will be opened next.</param>
	private void SwitchMenus(GameMenu current, GameMenu next) {
		if (!current.gameObject.activeInHierarchy) {
			next.Open();
			return;
		}
		if (simultaneousOpenClose) {
			current.Close();
			next.Open();
		}
		else current.Close(next.Open);
	}
}

[Serializable]
public struct GameMenuGroupEntry {
	public string name;
	public GameMenu menu;
}
}
