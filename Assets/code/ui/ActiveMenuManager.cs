using System;
using System.Collections.Generic;

using UnityEngine;

namespace ui {
[CreateAssetMenu(fileName = "menu-manager", menuName = "Managers/ActiveMenu")]
public class ActiveMenuManager : ScriptableObject {
	private readonly Stack<ActiveMenu> menuStack = new();

	public void NavigateTo(ActiveMenu menu) {
		if (menu == null) return;
		if (menuStack.Count >= 1 && IsCurrent(menu)) return;

		if (!menuStack.Peek().Deactivate(this))
			throw new MenuManagerException($"Tried to deactivate last menu on stack \"{menuStack.Peek().name}\", but another object claimed it.");
		if (!menu.Activate(this))
			throw new MenuManagerException($"Tried to activate menu \"{menu.name}\" but another object has it claimed.");
		menuStack.Push(menu);
	}

	public void Return() {
		if (menuStack.Count < 1) return;
		if (menuStack.Peek() != null) {
			if (!menuStack.Peek().Deactivate(this))
				throw new MenuManagerException($"Tried to deactivate last menu on stack \"{menuStack.Peek().name}\", but another object claimed it.");
			menuStack.Pop();
		}
		while (menuStack.Peek() == null) menuStack.Pop();
		if (!menuStack.Peek().Activate(this))
			throw new MenuManagerException($"Tried to activate menu \"{menuStack.Peek().name}\" but another object has it claimed.");
	}

	private bool IsCurrent(ActiveMenu menu) {
		return ReferenceEquals(menu, menuStack.Peek());
	}
}

public class MenuManagerException : Exception {
	public MenuManagerException(string message) : base(message) { }
}
}