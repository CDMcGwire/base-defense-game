using UnityEngine;

namespace demo {
public class AnimStateLogger : StateMachineBehaviour {
	public string label = "Default";

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		Debug.Log($"Entered Logger {label} on {animator.name}");
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		Debug.Log($"Exiting Logger {label} on {animator.name}");
	}
}
}