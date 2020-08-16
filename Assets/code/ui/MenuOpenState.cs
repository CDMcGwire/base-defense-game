using UnityEngine;

namespace ui {
public class MenuOpenState : StateMachineBehaviour {
	public GameMenu GameMenu { get; set; }

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		GameMenu.ChangeState(GameMenu.MenuState.Opening);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		GameMenu.ChangeState(GameMenu.MenuState.Open);
	}
}
}
