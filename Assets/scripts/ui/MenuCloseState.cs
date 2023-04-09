using UnityEngine;

namespace ui {
public class MenuCloseState : StateMachineBehaviour {
	public GameMenu GameMenu { get; set; }

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		GameMenu.ChangeState(GameMenu.MenuState.Closing);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		GameMenu.ChangeState(GameMenu.MenuState.Closed);
	}
}
}
