/*
 * Author: CharSui
 * Created On: 2024.05.26
 * Description: 方法来源于GPT，可以算出“水平”上玩家应该前往的位置，当然此位置是Position的，需要转换成RigidBody的移动,或者CharacterController的Move
 */
using UnityEngine;

public class CircularMovement : MonoBehaviour
{
    public float speed; // 速度（单位/秒）
    public Transform targetTransform; // 参考的Transform
    public float radius; // 圆柱的半径

    void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;
        MoveAlongCircle(speed, targetTransform, radius, deltaTime);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="transform"></param>
    /// <param name="radius"></param>
    /// <param name="deltaTime"></param>
    void MoveAlongCircle(float speed, Transform transform, float radius, float deltaTime)
    {
        // 计算弧长
        float distanceToMove = speed * deltaTime;

        // 计算弧度
        float angleToMove = distanceToMove / radius;

        // 计算新的位置
        Vector3 currentPosition = transform.position;
        float currentAngle = Mathf.Atan2(currentPosition.z, currentPosition.x);
        float newAngle = currentAngle + angleToMove;

        // 以新的角度计算新的位置
        float newX = radius * Mathf.Cos(newAngle);
        float newZ = radius * Mathf.Sin(newAngle);
        Vector3 newPosition = new Vector3(newX, currentPosition.y, newZ);

        // 更新Transform的位置
        transform.position = newPosition;
    }
}