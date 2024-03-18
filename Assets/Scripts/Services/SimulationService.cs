
using System.Collections.Generic;
using UnityEngine;

namespace Networking.ClientAutority
{
	public class SimulationService : ISimulation
	{
		private Dictionary<ulong, ISimulationObject> _simObjects =
			new Dictionary<ulong, ISimulationObject>();

		private const float COLLISION_DISTANCE = 1.5f;
		private const float SQR_COLLISION_DISTANCE = COLLISION_DISTANCE * COLLISION_DISTANCE;

		#region ISimulation

		public Vector3 GetValidRandomPosition()
		{
			Vector3 validPosition = Vector3.zero;
			bool foundPosition = true;
			do
			{
				Vector2 randomPosition = Random.insideUnitCircle;
				validPosition = new Vector3(randomPosition.x, randomPosition.y);
				validPosition = validPosition.normalized * 5f;

				foreach (KeyValuePair<ulong, ISimulationObject> kvp in _simObjects)
				{
					Transform simOther = kvp.Value.GetTransform();
					Vector3 distance = simOther.position - validPosition;
					if (distance.sqrMagnitude <= SQR_COLLISION_DISTANCE)
					{
						foundPosition = false;
						break;
					}
				}

			} while (!foundPosition);

			return validPosition;
		}

		public void SetSimulationObject(
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
				if (distance.sqrMagnitude > SQR_COLLISION_DISTANCE)
				{
					continue;
				}

				if (direction.sqrMagnitude > 0f)
				{
					direction.Normalize(); 
				} else
				{
					direction = Vector3.right;
				}

				distance.Normalize();
				
				float dotProduct = Vector3.Dot(distance, direction);
				if (dotProduct >= 0)
				{
					direction = -direction;
				}

				validatedPosition = simOther.position + direction * SQR_COLLISION_DISTANCE;
				return false;
			}

			return true;
		}

		#endregion
	}
}