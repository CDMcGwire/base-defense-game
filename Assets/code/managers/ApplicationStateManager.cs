using UnityEngine;

namespace managers {
public class ApplicationStateManager : MonoBehaviour {
	public void CloseGame() {
		Application.Quit();
	}
}
}
