using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField]
    private Vector3 _positionOffset = Vector3.zero;

    [SerializeField]
    private Vector3 _targetOffset = Vector3.zero;

    [SerializeField]
    private float _adjustmentSpeed = 10f;

    [SerializeField]
    private float _snapDistance = 0.5f;
    private float _sqrSnapDistance = 0f;

    private Transform _target;

    public void SetTarget(Transform target)
	{
        _target = target;
    }

	#region Monobehavior

	private void Awake()
	{
        _sqrSnapDistance = _snapDistance * _snapDistance;
    }

	// Update is called once per frame
	private void Update()
    {
        if (_target == null)
		{
            return;
		}

        Vector3 targetPosition = _target.position + _positionOffset;
        Vector3 distanceToTarget = targetPosition - transform.position;

        if (distanceToTarget.sqrMagnitude > _sqrSnapDistance)
		{
            transform.position += distanceToTarget.normalized * _adjustmentSpeed * Time.deltaTime;
		} else
		{
            transform.position = targetPosition;
		}

        Vector3 lookAtTarget = _target.position + _targetOffset;

        transform.LookAt(lookAtTarget, Vector3.up);
    }

	#endregion

}
