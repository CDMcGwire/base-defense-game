using UnityEngine;

namespace combat.weapon {
public class AnimAttackDriver : AttackDriver {
	private int triggerHash;

	private void Awake() {
		Debug.Assert(animator, $"Missing animator reference on AnimAttackDriver {name}");
		triggerHash = Animator.StringToHash(animTriggerName);
	}

	public override void Begin() {
		animator.SetTrigger(triggerHash);
	}

	public override void Release() {
		animator.ResetTrigger(triggerHash);
	}

	public void SignalAttackEnd() {
		Weapon.OnAttackEnd.Invoke();
	}

	public void PlayAttackSound() {
		if (audioSource && audioClip) audioSource.PlayOneShot(audioClip);
	}
#pragma warning disable 0649
	[SerializeField] private string animTriggerName;
	[SerializeField] private Animator animator;
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioClip audioClip;
#pragma warning restore 0649
}
}