/*
 * Author: CharSui
 * Created On: 2024.06.09
 * Description: 提供一个全局静态的方法，用于计算移动时需要水平环塔运动的情况
 */

using UnityEngine;

namespace GamePlay.PhysicMove
{
    public static class ObritMovementHelper
    {
        public static Vector3 GetHorizontalMovement(Vector3 originPosition, float speed, float radius, float deltaTime)
        {
            // 如果速度为0，直接返回当前位置
            if (speed == 0) return originPosition;

            // 计算弧长
            var distanceToMove = speed * deltaTime;

            // 计算弧度
            var angleToMove = distanceToMove / radius;

            // 计算当前角度
            Vector3 currentPosition = originPosition;
            var currentAngle = Mathf.Atan2(currentPosition.z, currentPosition.x);

            // 计算新的角度
            var newAngle = currentAngle + angleToMove;

            // 以新的角度计算新的位置
            var newX = radius * Mathf.Cos(newAngle);
            var newZ = radius * Mathf.Sin(newAngle);

            // 距离中心太近时，进行浮点数精度处理
            newX = Mathf.Abs(newX) < 1e-6 ? 0 : newX;
            newZ = Mathf.Abs(newZ) < 1e-6 ? 0 : newZ;

            var newPosition = new Vector3(newX, currentPosition.y, newZ);

            return newPosition;
        }

        /// <summary>
        /// 获取当前弧度
        /// </summary>
        /// <returns></returns>
        private static float GetAngle(Vector3 position)
        {
            // 防止除零错误
            if (position.x == 0 && position.z == 0)
            {
                Debug.LogError("位置在原点上，角度未定义。\n正常情况下不该有这种位置，请立即排查");
                return 0;
            }

            // 计算水平角度（弧度）
            float angleInRadians = Mathf.Atan2(position.z, position.x);
        
            // 将弧度转换为度数（可选）
            float angleInDegrees = angleInRadians * Mathf.Rad2Deg;

            return angleInDegrees;
        }
        
        /// <summary>
        /// 将物体Z轴对齐到目标方向（基于指定位置）
        /// </summary>
        /// <param name="objectPosition"></param>
        /// <param name="targetPosition"></param>
        // 方法：返回一个新的旋转，指向目标位置的方向（仅在XZ平面上）
        public static Quaternion GetQuaternionLockToTarget(Vector3 objectPosition, Vector3 targetPosition)
        {
            // 计算物体和目标之间在XZ平面上的方向向量
            Vector3 targetDirection = new Vector3(targetPosition.x - objectPosition.x, 0, targetPosition.z - objectPosition.z);
        
            if (targetDirection == Vector3.zero)
            {
                // 返回当前旋转以防止零向量情况
                Debug.LogError($"[{nameof(ObritMovementHelper)}]你输入了一个原点位置来求值");
                return default;
            }

            // 通过lookRotation计算四元数旋转，指向XZ方向
            Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
        
            // 获取lookRotation的欧拉角，只适用Y轴
            Vector3 eulerRotation = lookRotation.eulerAngles;

            // 返回一个新的欧拉角度，仅改变Y轴旋转
            return Quaternion.Euler(0, eulerRotation.y, 0);
        }
    }
}