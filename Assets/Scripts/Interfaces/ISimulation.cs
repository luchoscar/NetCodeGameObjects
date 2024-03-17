using UnityEngine;

public interface ISimulation
{
	void AddSimulationObject(
		ulong clientId, 
		ISimulationObject simulationObject
	);
	void RemoveSimulationObject(ulong clientId);
	bool ValidateMove(
		ulong clientId, 
		Vector3 nextPosition,
		out Vector3 validatedPosition
	);
}
