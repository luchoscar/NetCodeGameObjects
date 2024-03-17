using Networking.ClientAutority;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private ClientAuthoritySync _clientTransformSync;

	private bool _isDirty = false;
	private Vector3 _initialPosition = Vector3.zero;
	private Quaternion _initialRotation = Quaternion.identity;
	private Vector3 _initialLocalScale = Vector3.one;

	public void ForceResetTransform()
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

	#region Monobehavior

	private void Awake()
	{
		_clientTransformSync.AddTransformListener(OnTransformSyncEvent);
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
			transform.position += Vector3.one * 0.5f * Time.deltaTime;
			_isDirty = true;
		} else if (_clientTransformSync.Owner != ClientAuthoritySync.OwnerType.Server)
		{
			transform.position = _clientTransformSync.GetPositionSync();
			transform.rotation = _clientTransformSync.GetRotationSync();
			transform.localScale = _clientTransformSync.GetLocalSync();
		}
	}

	private void FixedUpdate()
	{
		if (_isDirty)
		{
			_clientTransformSync.SyncServerWithClient(
				transform.position,
				transform.rotation,
				transform.localScale
			);

			_isDirty = false;
		}	
	}

	private void OnDestroy()
	{
		if (_clientTransformSync != null)
		{
			_clientTransformSync.RemoveTransformListener(OnTransformSyncEvent);
		}
	}

	#endregion

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
}
