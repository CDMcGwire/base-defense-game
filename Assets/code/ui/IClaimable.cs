using UnityEngine;

public interface IClaimable {
	bool Take(Object claimer);
	void Release(Object claimer);
}