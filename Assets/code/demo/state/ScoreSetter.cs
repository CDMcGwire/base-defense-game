using System;
using data.refvalues;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace demo.state {
public class ScoreSetter : MonoBehaviour {
	public RefValue<long> score;
	public TMP_InputField inputField;

	private Action<long, long> scoreChangeAction;
	private UnityAction<string> inputAction;

	private void OnEnable() {
		scoreChangeAction = (previous, current) => inputField.text = $"{current:N}";
		inputAction = value => {
			if (double.TryParse(value, out var number))
				score.Value.Current = (long) number;
			else
				Debug.LogError("Failed to enter a number!");
		};
		
		score.Value.OnChanged += scoreChangeAction;
		inputField.onEndEdit.AddListener(inputAction);
	}

	private void OnDisable() {
		score.Value.OnChanged -= scoreChangeAction;
		inputField.onEndEdit.RemoveListener(inputAction);
	}

	public void SetScore(string value) {
		if (long.TryParse(value, out var number))
			score.Value.Current = number;
	}
}
}
