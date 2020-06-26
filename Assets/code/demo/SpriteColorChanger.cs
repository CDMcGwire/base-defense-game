using combat.components;
using UnityEngine;

namespace demo {
public class SpriteColorChanger : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private SpriteRenderer targetSpriteRenderer;
	[SerializeField] private Color targetColor;
#pragma warning restore 0649

	private Color originalColor;

	private void Start() => originalColor = targetSpriteRenderer.color;

	public void ReceiveDamage(DamageReport report) {
		var percentRemaining = (float)report.currentHealth / report.maxHealth;
		var newColor = Color.Lerp(targetColor, originalColor, percentRemaining);
		targetSpriteRenderer.color = newColor;
	}
}
}
