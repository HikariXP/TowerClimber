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

    public void OnCollisionStay(Collision collisionInfo)
    {
        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            // 获取碰撞的法线
            Vector3 normal = contact.normal;

            // 检查法线是否垂直于地面（假设Y轴为垂直方向）
            // 如果法线与垂直方向（Vector3.up）之间的点积接近1或-1，则说明几乎是垂直的。
            // 可以设置一个阈值来判断是否大致垂直，例如0.9或更高。
            if (Mathf.Abs(Vector3.Dot(normal, Vector3.up)) > 0.9f)
            {
                var rbom = contact.otherCollider.GetComponent<RigidBodyObritMove>();
                if(!rbom)return;
                
                rbom.SetEnvironmentVelocity(Vector2.right * speed);
                // var temp = rbom.GetVelocity();
                // temp.x += speed;
                // rbom.SetVelocity(temp);
            }
            else
            {
                Debug.Log("碰撞法线非垂直");
            }
        }

        // 旧方案对垂直无效，且对于水平碰撞的时候，玩家会被弹飞。
        // var rbom = collisionInfo.body.GetComponent<RigidBodyObritMove>();
        // var temp = rbom.GetVelocity();
        // temp.x += speed;
        // rbom.SetVelocity(temp);
    }
}
