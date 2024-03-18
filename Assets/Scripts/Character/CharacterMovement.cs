using Networking.ClientAutority;
using UnityEngine;

public class CharacterMovement : MonoBehaviour, IPlayableCharacter, ISimulationObject
{
    [SerializeField]
    private ClientAuthoritySync _clientTransformSync;

	[SerializeField]
	private float _movementSpeed = 0.5f;
	
	private Vector3 _direction = Vector3.zero;

	private IInput _input;
	private ITicker _ticker;
	
	private bool _forceServerSync = false;

	#region Monobehavior

	private void Start()
	{
		if (_clientTransformSync.IsOwner)
		{
			Camera.main.GetComponent<FollowTarget>().SetTarget(transform);
		}
	}

	private void OnDestroy()
	{
		_input?.RemoveInputListener(OnInputEvent);
		_ticker?.RemoveListener(OnTickEvent);
	}

	#endregion

	#region ISimulationObject

	public Transform GetTransform() => this.transform;

	#endregion

	#region ITicker listeners

	private void OnTickEvent(float deltaTime)
	{
		if (!_forceServerSync && _clientTransformSync.IsOwner)
		{
			transform.position += _direction * _movementSpeed * deltaTime;
			_clientTransformSync.SyncServerWithClient(
				transform.position,
				transform.rotation,
				transform.localScale
			);
		}
		else 
		{
			transform.position = _clientTransformSync.GetPositionSync();
			transform.rotation = _clientTransformSync.GetRotationSync();
			transform.localScale = _clientTransformSync.GetLocalSync();

			_direction = Vector3.zero;
			_forceServerSync = false;
		}
	}

	#endregion

	#region IPlayableCharacter

	public void Initialize(IServiceProvider provider)
	{
		if (_clientTransformSync.IsOwner
			&& _clientTransformSync.Process != ProcessType.Server
		)
		{
			_input = provider.GetService<IInput>();
			_input.AddInputListener(OnInputEvent);
		}

		_ticker = provider.GetService<ITicker>();
		_ticker.AddListener(OnTickEvent);

		_clientTransformSync.SetSyncTransformCallback(OnServerTransformSyncCallback);
	}

	#endregion

	#region Network Sync

	private void OnServerTransformSyncCallback()
	{
		_forceServerSync = true;
	}

	#endregion

	#region IInput listeners

	private void OnInputEvent(KeyInput inputCollected)
	{
		_direction = Vector3.zero;

		foreach (KeyInput input in System.Enum.GetValues(typeof(KeyInput)))
		{
			KeyInput keyInput = input & inputCollected;

			switch (keyInput)
			{
				case KeyInput.Up:
					_direction += Vector3.up;
					break;

				case KeyInput.Down:
					_direction -= Vector3.up;
					break;

				case KeyInput.Left:
					_direction -= Vector3.right;
					break;

				case KeyInput.Right:
					_direction += Vector3.right;
					break;
			}
		}
	}

	#endregion
}
