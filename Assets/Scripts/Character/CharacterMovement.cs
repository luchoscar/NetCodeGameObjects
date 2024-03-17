using Networking.ClientAutority;
using UnityEngine;

public interface IPlayableCharacter
{
	void SetupAsLocalPlayer();
}

public class CharacterMovement : MonoBehaviour, IPlayableCharacter
{
    [SerializeField]
    private ClientAuthoritySync _clientTransformSync;

	[SerializeField]
	private float _movementSpeed = 0.5f;
	
	private Vector3 _direction = Vector3.zero;
	private Vector3 _velocity = Vector3.zero;

	private InputHandler _inputHandler;

	private bool _isDirty = false;
	private Vector3 _initialPosition = Vector3.zero;
	private Quaternion _initialRotation = Quaternion.identity;
	private Vector3 _initialLocalScale = Vector3.one;

	#region Monobehavior

	private void Awake()
	{
		_clientTransformSync.SetSyncTransformCallback(OnTransformSyncEvent);
	}

	private void Start()
	{
		if (_clientTransformSync.IsOwner)
		{
			Camera.main.GetComponent<FollowTarget>().SetTarget(transform);
		}

		_initialPosition = transform.position;
		_initialRotation = transform.rotation;
		_initialLocalScale = transform.localScale;
	}

	private void Update()
	{
		if (_clientTransformSync.IsOwner)
		{
			transform.position += _direction * _movementSpeed * Time.deltaTime;
			_isDirty = true;
		} else if (_clientTransformSync.Process != ClientAuthoritySync.ProcessType.Server)
		{
			transform.position = _clientTransformSync.GetPositionSync();
			transform.rotation = _clientTransformSync.GetRotationSync();
			transform.localScale = _clientTransformSync.GetLocalSync();
		}

		if (_isDirty)
		{
			_clientTransformSync.SyncServerWithClient(
				transform.position,
				transform.rotation,
				transform.localScale,
				Time.deltaTime
			);

			_isDirty = false;
		}	
	}

	private void OnDestroy()
	{
		if (_inputHandler != null)
		{
			_inputHandler.RemoveInputListener(OnInputChangeEvent);
		}
	}

	#endregion

	#region IPlayableCharacter

	public void SetupAsLocalPlayer()
	{
		_inputHandler = gameObject.AddComponent<InputHandler>();
		_inputHandler.AddInputListener(OnInputChangeEvent);
	}

	#endregion

	#region Network Sync

	private void OnTransformSyncEvent(
		Vector3 position,
		Quaternion rotation, 
		Vector3 localScale
	)
	{
		transform.position = position;
		transform.rotation = rotation;
		transform.localScale = localScale;
	}

	public void ForceServerResetTransform()
	{
		if (_clientTransformSync.Process == ClientAuthoritySync.ProcessType.Server)
		{
			transform.position = _initialPosition;
			transform.rotation = _initialRotation;
			transform.localScale = _initialLocalScale;

			_clientTransformSync.SyncClientWithServer(
				transform.position,
				transform.rotation,
				transform.localScale
			);
		}
	}

	public void ForceClientResetTransform()
	{
		transform.position = _initialPosition;
		transform.rotation = _initialRotation;
		transform.localScale = _initialLocalScale;

		_clientTransformSync.SyncServerWithClient(
			transform.position,
			transform.rotation,
			transform.localScale,
			0f
		);
	}

	#endregion
	
	private void OnInputChangeEvent(KeyInput inputCollected)
	{
		_direction = Vector3.zero;

		foreach (KeyInput input in System.Enum.GetValues(typeof(KeyInput)))
		{
			KeyInput keyInput = input & inputCollected;
			Debug.Log($"keyInput: {keyInput}");
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
}
