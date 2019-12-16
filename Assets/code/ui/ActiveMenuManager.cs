using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "menu-manager", menuName = "Managers/ActiveMenu")]
public class ActiveMenuManager : ScriptableObject {
	private readonly Stack<ClaimedMenu> menuStack = new Stack<ClaimedMenu>();

	public void NavigateTo(IActiveMenu menu, IClaimable claim) {
		if (menu == null) return;
		if (menuStack.Count >= 1 && IsCurrent(menu)) return;

		menuStack.Peek().menu.Deactivate();
		menu.Activate();
		menuStack.Push(new ClaimedMenu(menu, claim));
	}

	public void Return() {
		if (menuStack.Count < 1) return;
		if (menuStack.Peek() != null) {
			menuStack.Peek().menu.Deactivate();
			menuStack.Pop();
		}
		// TODO: If I'm using Interface references, how do I managed dead references?
		while (menuStack.Peek() == null) menuStack.Pop();
		menuStack.Peek().menu.Activate();
	}

	private bool IsCurrent(IActiveMenu menu) => ReferenceEquals(menu, menuStack.Peek().menu);
	
	private struct ClaimedMenu {
		public readonly IActiveMenu menu;
		public readonly IClaimable claim;

		public ClaimedMenu(IActiveMenu menu, IClaimable claim) {
			this.menu = menu;
			this.claim = claim;
		}
	}
}