using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace ui {
/// <summary>
///   <para>The GameMenu component provides an API for handling a GameObject as a typical
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
///   calling an API method.</para>
/// </summary>
public class GameMenu : MonoBehaviour {
	private static readonly int AnimFloatSpeed = Animator.StringToHash("speed");
	private static readonly int AnimTrigClose = Animator.StringToHash("close");
	private static readonly int AnimTrigSkip = Animator.StringToHash("skip");

#pragma warning disable 0649
	[Tooltip("Speed modifier for the menu animations"), SerializeField]
	private float animSpeedMult = 1.0f;

	[Tooltip("Should this menu stay in an active state if the game object is disabled directly?"), SerializeField]
	private bool stayOpenOnShortCircuit = true;

	[Tooltip("Should the owning game object be destroyed on close?"), SerializeField]
	private bool destroyOnClose;

	[SerializeField] private UnityEvent onOpening = new();
	[SerializeField] private UnityEvent onOpened = new();
	[SerializeField] private UnityEvent onClosing = new();
	[SerializeField] private UnityEvent onClosed = new();
#pragma warning restore 0649

	private Animator animator;
	private Action onNextCloseCallback;
	private bool shouldCycle;

	[UsedImplicitly] public MenuState State { get; private set; } = MenuState.Closed;

	public void ChangeState(MenuState value) {
		switch (value) {
			case MenuState.Closed:
				if (State == MenuState.Closing) {
					State = value;
					OnClosed.Invoke();
				}
				break;
			case MenuState.Opening:
				if (State == MenuState.Closed) {
					State = value;
					OnOpening.Invoke();
				}
				break;
			case MenuState.Open:
				if (State == MenuState.Opening) {
					State = value;
					OnOpened.Invoke();
				}
				break;
			case MenuState.Closing:
				if (State == MenuState.Open) {
					State = value;
					OnOpening.Invoke();
				}
				break;
			default: throw new ArgumentOutOfRangeException(nameof(value), value, null);
		}
	}

	[UsedImplicitly] public UnityEvent OnOpening => onOpening;
	[UsedImplicitly] public UnityEvent OnOpened => onOpened;
	[UsedImplicitly] public UnityEvent OnClosing => onClosing;
	[UsedImplicitly] public UnityEvent OnClosed => onClosed;

	private void Awake() {
		animator = GetComponent<Animator>();
		OnClosed.AddListener(Deactivate);
	}

	private void OnEnable() {
		SetupAnimator();
		Initialize();
	}

	private void OnDisable() {
		// Resolve short circuit cases
		CleanupShortCircuit();
	}

	[UsedImplicitly]
	public void Open() {
		gameObject.SetActive(true);
	}

	[UsedImplicitly]
	public void Close(bool immediate) {
		// If there is an animator present and immediate mode wasn't set, let the animator drive the closing logic.
		if (animator && !immediate) {
			animator.SetTrigger(AnimTrigClose);
		}
		else {
			// If closing immediate, then fast forward the opening operation as well.
			if (immediate && State == MenuState.Opening)
				ChangeState(MenuState.Open);
			ChangeState(MenuState.Closing);
			ChangeState(MenuState.Closed);
		}
	}

	[UsedImplicitly]
	public void Close() => Close(false);

	public void Close(Action callback, bool immediate = false) {
		onNextCloseCallback += callback;
		Close(immediate);
	}

	[UsedImplicitly]
	public void Cycle() {
		shouldCycle = true;
		Close();
	}

	public void Cycle(Action callback) {
		shouldCycle = true;
		Close(callback);
	}

	/// <summary>
	/// Call mid animation to fast-forward the menu opening or closing.
	/// </summary>
	[UsedImplicitly]
	public void SkipAnim() {
		if (!animator) return;
		animator.SetTrigger(AnimTrigSkip);
	}

	/// <summary>
	/// Setup logic that should be applied every time the menu is enabled or cycled.
	/// </summary>
	private void Initialize() {
		// If animated, initialize animation parameters. Opening animation should play automatically.
		if (animator) {
			animator.SetFloat(AnimFloatSpeed, animSpeedMult);
		}
		// Otherwise invoke relevant events immediately.
		else {
			ChangeState(MenuState.Opening);
			ChangeState(MenuState.Open);
		}
	}

	/// <summary>
	/// Ensures the menu's internal state is correct when the game object is disabled directly.
	/// </summary>
	private void CleanupShortCircuit() {
		if (State == MenuState.Opening)
			ChangeState(MenuState.Open);
		if (State == MenuState.Open && !stayOpenOnShortCircuit)
			Close(true);
		else if (State == MenuState.Closing)
			ChangeState(MenuState.Closed);
	}

	/// <summary>
	/// The final set of logic called during the close process. Handles disabling, destroying, or cycling as the menu is
	/// configured.
	/// </summary>
	private void Deactivate() {
		// Trigger the one-time closing callback and clear.
		onNextCloseCallback?.Invoke();
		onNextCloseCallback = null;
		// If cycling, the game object is left active but this script is reinitialized.
		if (shouldCycle) {
			shouldCycle = false;
			Initialize();
		}
		// Otherwise either disable or destroy the menu based on config.
		else {
			if (destroyOnClose) Destroy(gameObject);
			else gameObject.SetActive(false);
		}
	}

	private void SetupAnimator() {
		if (animator == null) return;

		var openState = animator.GetBehaviour<MenuOpenState>();
		Debug.Assert(
			openState != null,
			"GameMenu contains an Animator but assigned controller has no MenuOpenState behaviour"
		);

		var closeState = animator.GetBehaviour<MenuCloseState>();
		Debug.Assert(
			closeState != null,
			"GameMenu contains an Animator but assigned controller has no MenuCloseState behaviour"
		);

		openState.GameMenu = this;
		closeState.GameMenu = this;
	}

	[Serializable]
	public enum MenuState {
		Closed,
		Opening,
		Open,
		Closing,
	}
}
}
