/*
 * Author: CharSui
 * Created On: 2024.06.10
 * Description: 物理自转
 */
using UnityEngine;

public class RigidBodySelfRotate : MonoBehaviour
{
    // 每秒转动角度
    public float angleSpeed = 30f;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // 计算每帧的旋转角度
        float rotationAngle = angleSpeed * Time.fixedDeltaTime;

        // 计算新的旋转
        Quaternion deltaRotation = Quaternion.Euler(0, rotationAngle, 0);

        // 使用 MoveRotation 方法进行旋转
        _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
    }
}
