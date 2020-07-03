using System;

using UnityEngine;

namespace ui {
public class MenuOpenState : StateMachineBehaviour {
	public event Action OnOpening;
	public event Action OnOpened;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		OnOpening?.Invoke();
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		OnOpened?.Invoke();
	}
}
}