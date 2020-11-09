using UnityEngine;
using UnityEngine.Events;

namespace utility {
public class OnStartEvent : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private UnityEvent onStart;
#pragma warning restore 0649

	private void Start() => onStart.Invoke();
}
}