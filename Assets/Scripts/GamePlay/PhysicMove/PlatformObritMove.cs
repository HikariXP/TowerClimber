/*
 * Author: CharSui
 * Created On: 2024.06.09
 * Description:用于处理一些平台移动的情况。 
 */

using System;
using GamePlay.PhysicMove;
using UnityEngine;

public class PlatformObritMove : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private Transform _transform;

    public float speed = 3f;

    public float obritRadius = 11f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = transform;
    }

    private void FixedUpdate()
    {
        var position = _transform.position;
        
        var tempMovement = ObritMovementHelper.GetHorizontalMovement(position, speed, obritRadius, Time.deltaTime);
        var tempRotation = ObritMovementHelper.GetQuaternionLockToTarget(position,Vector3.zero);
        
        _rigidbody.MovePosition(tempMovement);
        _rigidbody.MoveRotation(tempRotation);
    }
}
