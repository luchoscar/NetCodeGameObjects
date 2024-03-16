using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField]
    private Vector3 _positionOffset = Vector3.zero;

    [SerializeField]
    private float _adjustmentSpeed = 10f;

    [SerializeField]
    private float _snapDistance = 0.5f;

    private Transform _target;

    public void SetTarget(Transform target)
	{
        _target = target;
        transform.LookAt(_target, Vector3.up);
    }

    // Update is called once per frame
    private void Update()
    {
        if (_target == null)
		{
            return;
		}

        Vector3 targetPosition = _target.position -  _positionOffset;
        Vector3 distanceToTarget = targetPosition - transform.position;
        if (distanceToTarget.magnitude > _snapDistance)
		{
            transform.position += distanceToTarget.normalized * _adjustmentSpeed * Time.deltaTime;
		} else
		{
            transform.position = targetPosition;
		}

        transform.LookAt(_target, Vector3.up);

        
    }
}
