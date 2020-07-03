using System;

using UnityEngine;
using UnityEngine.Events;

namespace ui {
/// <summary>
///   The GameMenu component provides an API for handling a GameObject as a typical
///   in-game menu. Any system looking to enable the menu would call into the
///   Open() and then the Close() method to deactivate, or alternatively the
///   Cycle() method to have the menu immediately reopen after firing off its
///   lifecycle events. Useful when reusing a single menu instance that is
///   initialized on open. If an animator is present on the same GameObject, this
///   component will look for MenuOpenState and MenuCloseState
///   StateMachineBehaviours on the animator. If found it will then delegate its
///   event invocations to those classes, letting the animation state machine drive
///   the lifecycle while the API methods simply trigger transitions on the
///   animator. Otherwise, the relevant events will be invoked immediately upon
///   calling an API method.
/// </summary>
public class GameMenu : MonoBehaviour {
	private static readonly int AnimFloatSpeed = Animator.StringToHash("speed");
	private static readonly int AnimTrigClose = Animator.StringToHash("close");
	private static readonly int AnimTrigSkip = Animator.StringToHash("skip");

	private Animator animator;

	[Tooltip("Speed modifier for the menu animations")] [SerializeField]
	private float animSpeedMult = 1.0f;
	private bool beganClose;

	[Tooltip("Should the owning game object be destroyed on close?")] [SerializeField]
	private bool destroyOnClose;
	private bool forcedClosed = true;
	[SerializeField] private UnityEvent onClosed = new UnityEvent();
	[SerializeField] private UnityEvent onClosing = new UnityEvent();

	private Action onNextCloseCallback;
	[SerializeField] private UnityEvent onOpened = new UnityEvent();

	[SerializeField] private UnityEvent onOpening = new UnityEvent();
	private bool shouldCycle;

	public UnityEvent OnOpening => onOpening;
	public UnityEvent OnOpened => onOpened;
	public UnityEvent OnClosing => onClosing;
	public UnityEvent OnClosed => onClosed;

	private void Awake() {
		animator = GetComponent<Animator>();
		if (animator != null) SetupAnimator();
		OnClosing.AddListener(() => beganClose = true);
		OnClosed.AddListener(Deactivate);
	}

	private void OnEnable() {
		Init();
	}

	private void OnDisable() {
		if (forcedClosed) {
			if (beganClose) OnClosing.Invoke();
			OnClosed.Invoke();
		}
		beganClose = false;
		forcedClosed = true;
	}

	public void Open() {
		gameObject.SetActive(true);
	}

	public void Close() {
		if (animator) {
			animator.SetTrigger(AnimTrigClose);
		}
		else {
			OnClosing.Invoke();
			OnClosed.Invoke();
		}
	}

	public void Close(Action callback) {
		onNextCloseCallback += callback;
		Close();
	}

	public void Cycle() {
		shouldCycle = true;
		Close();
	}

	public void Cycle(Action callback) {
		shouldCycle = true;
		Close(callback);
	}

	public void SkipAnim() {
		if (!animator) return;
		animator.SetTrigger(AnimTrigSkip);
	}

	private void Init() {
		// If animated, initialize animation parameters.
		if (animator) {
			animator.SetFloat(AnimFloatSpeed, animSpeedMult);
		}
		// Otherwise invoke relevant events immediately.
		else {
			OnOpening.Invoke();
			OnOpened.Invoke();
		}
	}

	private void Deactivate() {
		// Set flag so OnDisable know the menu is closing normally.
		forcedClosed = false;
		// If cycling, the game object is left active but this script is reinitialized.
		if (shouldCycle) {
			shouldCycle = false;
			onNextCloseCallback?.Invoke();
			Init();
		}
		// Otherwise either disable or destroy the menu based on config.
		else {
			if (destroyOnClose) Destroy(gameObject);
			else gameObject.SetActive(false);
			onNextCloseCallback?.Invoke();
		}
		// Clear the single time close callback.
		onNextCloseCallback = null;
	}

	private void SetupAnimator() {
		if (animator == null)
			throw new Exception($"Trying to setup animator on menu {name} but no animator component was populated.");

		var openState = animator.GetBehaviour<MenuOpenState>();
		Debug.Assert(openState != null,
			"GameMenu contains an Animator but assigned controller has no MenuOpenState behaviour");

		var closeState = animator.GetBehaviour<MenuCloseState>();
		Debug.Assert(closeState != null,
			"GameMenu contains an Animator but assigned controller has no MenuCloseState behaviour");

		openState.OnOpening += OnOpening.Invoke;
		openState.OnOpened += OnOpened.Invoke;
		closeState.OnClosing += OnClosing.Invoke;
		closeState.OnClosed += OnClosed.Invoke;
	}
}
}