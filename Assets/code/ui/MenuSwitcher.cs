using System;
using System.Collections.Generic;
using UnityEngine;

namespace ui {
public class MenuSwitcher : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private string initialMenuName = "";
	[SerializeField] private bool rootOnDisable = true;
	[SerializeField] private GameMenuGroupEntry[] menus;
#pragma warning restore 0649

	private readonly Dictionary<string, GameMenu> menuLookup = new Dictionary<string, GameMenu>();
	private readonly LinkedList<string> history = new LinkedList<string>();

	private string currentMenuName = "";

#if UNITY_EDITOR
	private void OnValidate() {
		initialMenuName = initialMenuName.Trim();
		if (Application.IsPlaying(this) && menuLookup.Count > 0) CompileMenuLookup();
	}
#endif

	private void OnEnable() {
		CompileMenuLookup();
		if (initialMenuName.Length > 0) Change(initialMenuName);
	}

	private void OnDisable() {
		if (rootOnDisable) Root();
	}

	public void Change(string menuName) {
		var cleanMenuName = menuName.Trim();
		if (!menuLookup.ContainsKey(cleanMenuName)) {
			Debug.LogWarning($"Tried to change to menu {cleanMenuName} from group {name} but it was not found");
			return;
		}
		if (currentMenuName == cleanMenuName) return;

		if (currentMenuName.Length < 1) {
			menuLookup[cleanMenuName].Open();
			currentMenuName = cleanMenuName;
		}
		else {
			var currentMenu = menuLookup[currentMenuName];
			var nextMenu = menuLookup[cleanMenuName];
			currentMenu.Close(nextMenu.Open);
		}
		_ = history.Remove(cleanMenuName);
		history.AddLast(cleanMenuName);
		currentMenuName = cleanMenuName;
	}

	public void Return() {
		if (history.Count < 1) return;
		
		var currentMenu = menuLookup[currentMenuName];
		currentMenuName = history.Last.Value;
		
		if (currentMenuName.Length < 1) currentMenu.Close();
		else {
			var lastMenu = menuLookup[currentMenuName];
			currentMenu.Close(lastMenu.Open);
		}
		history.RemoveLast();
	}

	public void Root() {
		if (history.Count < 1) return;
		
		var currentMenu = menuLookup[currentMenuName];

		if (initialMenuName.Length < 1) currentMenu.Close();
		else {
			currentMenuName = initialMenuName;
			var initialMenu = menuLookup[currentMenuName];
			currentMenu.Close(initialMenu.Open);
		}
		history.Clear();
	}

	private void CompileMenuLookup() {
		menuLookup.Clear();
		foreach (var entry in menus) {
			menuLookup[entry.name.Trim()] = entry.menu;
			entry.menu.gameObject.SetActive(false);
		}
	}
}

[Serializable]
public struct GameMenuGroupEntry {
	public string name;
	public GameMenu menu;
}
}