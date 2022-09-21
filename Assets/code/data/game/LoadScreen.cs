using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace data.game {
/// <summary>
/// Handles creating and maintaining a scene load operation. Should be used to
/// display graphics over a scene transition.
/// </summary>
public class LoadScreen : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private bool finishImmediately;
	[SerializeField] private UnityEvent onLoadComplete = new();
#pragma warning restore 0649

	private AsyncOperation operation;

	/// <summary>
	/// Whether or not the load operation has completed.
	/// </summary>
	public bool IsDone => operation?.isDone ?? false;

	/// <summary>
	/// The current progress of the load operation.
	/// </summary>
	public float Progress => operation?.progress ?? 0f;

	/// <summary>
	/// Event to trigger upon completion of the load operation.
	/// </summary>
	public UnityEvent OnLoadComplete => onLoadComplete;

	private void Awake() {
		DontDestroyOnLoad(this);
	}

	/// <summary>
	/// Call to enable the load screen and begin the transitioning to the target
	/// scene.
	/// </summary>
	/// <param name="nextScene">The loadable name of the scene to transition to.</param>
	public void StartLoad(string nextScene) {
		gameObject.SetActive(true);
		StartCoroutine(LoadLevelAsync(nextScene));
	}

	/// <summary>
	/// Call to tell a scene load in progress that it can finish. Used to keep
	/// the screen covered until ready to swap over.
	/// </summary>
	[UsedImplicitly]
	public void SetReadyToTransition() {
		operation.allowSceneActivation = true;
	}

	private IEnumerator LoadLevelAsync(string level) {
		yield return null;
		operation = SceneManager.LoadSceneAsync(level);
		operation.allowSceneActivation = finishImmediately;
		while (!operation.isDone)
			yield return null;
		onLoadComplete.Invoke();
	}
}
}