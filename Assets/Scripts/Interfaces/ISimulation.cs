using UnityEngine;

namespace Networking.ClientAutority
{
	public interface ISimulation
	{
		Vector3 GetValidRandomPosition();
		void SetSimulationObject(
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
}