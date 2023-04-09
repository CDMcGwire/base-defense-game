using System;
using data.refconst;
using data.service;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace data.game {
[CreateAssetMenu(fileName = "scene-loader-service", menuName = "Service/Scene Loader", order = 0)]
public class SceneLoaderService : Service {
#pragma warning disable 0649
	[SerializeField] private LoadScreen loadScreenPrefab;
	[SerializeField] private RefConst<string> fallbackScene;
#pragma warning restore 0649
	
	private LoadScreen ActiveLoadScreen { get; set; }

	private bool AlreadyLoading
		=> ActiveLoadScreen != null && ActiveLoadScreen.gameObject.activeInHierarchy;

	private string NoFallbackError 
		=> $"No suitable fallback scene has been designated for SceneLoader {name}";

	private string AlreadyAtFallbackError
		=> $"Progress tracker tried to progress to an invalid scene, but the fallback scene [{fallbackScene}] is already loaded.";

	private string AlreadyLoadingWarning
		=> $"Tried to end scene from progress tracker {name}, but it is already in the process of closing a scene.";

	public void LoadScene(string sceneName) {
		if (AlreadyLoading) {
			Debug.LogWarning(AlreadyLoadingWarning);
			return;
		}
		if (string.IsNullOrWhiteSpace(sceneName)) {
			if (string.IsNullOrWhiteSpace(fallbackScene.Value))
				throw new SceneLoadException(NoFallbackError);
			if (SceneManager.GetActiveScene().name == fallbackScene.Value)
				throw new ProgressTrackerException(AlreadyAtFallbackError);
			BeginSceneTransition(fallbackScene.Value);
		}
		BeginSceneTransition(sceneName);
	}

	/// <summary>
	/// Starts the process of transitioning to the given scene by creating a load
	/// screen and initializing it.
	/// </summary>
	/// <param name="sceneName">The scene to transition to.</param>
	private void BeginSceneTransition(string sceneName) {
		// Create a load screen object and start the load coroutine on it.
		if (ActiveLoadScreen == null) {
			ActiveLoadScreen = Instantiate(
				loadScreenPrefab,
				Vector3.zero,
				Quaternion.identity
			);
		}
		ActiveLoadScreen.StartLoad(sceneName);
	}
}

public class SceneLoadException : Exception {
	public SceneLoadException(string message) : base(message) { }
}
}