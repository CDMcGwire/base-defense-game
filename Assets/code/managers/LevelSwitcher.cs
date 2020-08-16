using UnityEngine;
using UnityEngine.SceneManagement;

namespace managers {
public class LevelSwitcher : MonoBehaviour {
	public void ReturnToMain() {
		SceneManager.LoadScene(0);
	}

	public void Load(string level) {
		SceneManager.LoadScene(level);
	}

	public void Load(int level) {
		SceneManager.LoadScene(level);
	}
}
}
