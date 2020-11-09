using System;
using UnityEngine;

namespace utility.math {
[Serializable]
public struct ScalingValue : ISerializationCallbackReceiver {
#pragma warning disable 0649
	[SerializeField] private float factor;
	[SerializeField] private ScaleFunction function;
	[SerializeField] private bool decay;
#pragma warning restore 0649

	public float At(float target, float t)
		=> target + (decay ? -1 : 1) * CalcGrowth(t);

	private float CalcGrowth(float t) {
		if (factor <= 0) return 0;
		switch (function) {
			case ScaleFunction.Linear: return factor / 10 * t;
			case ScaleFunction.Exponential: return Mathf.Pow(t / 10, factor);
			case ScaleFunction.Logarithmic: return factor * (Mathf.Log(t / 10 + 1) / Mathf.Log(2));
			default: throw new ArgumentOutOfRangeException();
		}
	}

	public void OnBeforeSerialize() { }

	public void OnAfterDeserialize()
		=> factor = Mathf.Max(factor, function == ScaleFunction.Exponential ? 1 : 0);
}

[Serializable]
public enum ScaleFunction {
	Linear, // Every 10 days, the amount will increase by F
	Exponential, // Every 10 days, the amount will multiply by F
	Logarithmic, // After 10 days, the amount will increase by F, every 10 days after it will decrease by roughly half the last increase.
}
}