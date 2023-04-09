using UnityEngine;

namespace combat.movement {
public class AnimMoveController : MonoBehaviour {
	private void Start() {
		Debug.Assert(animator, $"No animator found for AnimMoveController to manage on {name}");
		animator.SetFloat(speedParam, speed);
	}
#pragma warning disable 0649
	[SerializeField] private Animator animator;
	[SerializeField] private float speed = 1f;
	[SerializeField] private string speedParam = "speed";
#pragma warning restore 0649
}
}