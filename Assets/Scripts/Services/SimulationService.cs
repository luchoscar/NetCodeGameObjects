
using System.Collections.Generic;
using UnityEngine;

public class SimulationService : ISimulation
{
	private Dictionary<ulong, ISimulationObject> _simObjects = 
		new Dictionary<ulong, ISimulationObject>();

	private const float COLLISION_DISTANCE = 1.5f;
	private const float SQR_COLLISION_DISTANCE = COLLISION_DISTANCE * COLLISION_DISTANCE;

	public void AddSimulationObject(
		ulong clientId, 
		ISimulationObject simulationObject
	)
	{
		if (simulationObject != null)
		{
			_simObjects.Add(clientId, simulationObject);
		}
	}

	public void RemoveSimulationObject(ulong clientId)
	{
		_simObjects.Remove(clientId);
	}

	public bool ValidateMove(
		ulong clientId, 
		Vector3 nextPosition,
		out Vector3 validatedPosition
	)
	{
		validatedPosition = nextPosition;

		if (!_simObjects.TryGetValue(clientId, out ISimulationObject simObject)
			|| simObject == null
		)
		{
			return true;
		}

		Transform simTransform = simObject.GetTransform();
		Vector3 direction = nextPosition - simTransform.position;

		foreach (KeyValuePair<ulong, ISimulationObject> kvp in _simObjects)
		{
			if (kvp.Key == clientId || kvp.Value == null)
			{
				continue;
			}

			Transform simOther = kvp.Value.GetTransform();
			Vector3 distance = simOther.position - simTransform.position;
			if (distance.sqrMagnitude >= SQR_COLLISION_DISTANCE)
			{
				continue;
			}

			distance.Normalize();
			direction.Normalize();
			if (Vector3.Dot(distance, direction) > 0)
			{
				continue;
			}

			direction = -direction;

			validatedPosition = simOther.position + direction * COLLISION_DISTANCE;
			return false;
		}

		return true;
	}
}
