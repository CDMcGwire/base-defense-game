using System;
using UnityEngine;

namespace ui {
public class MenuCloseState : StateMachineBehaviour {
	public event Action OnClosing;
	public event Action OnClosed;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) => OnClosing?.Invoke();

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) => OnClosed?.Invoke();
}
}