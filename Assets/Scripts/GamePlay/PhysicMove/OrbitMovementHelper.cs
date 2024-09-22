/*
 * Author: CharSui
 * Created On: 2024.06.09
 * Description: 提供一个全局静态的方法，用于计算移动时需要水平环塔运动的情况
 */

using UnityEngine;

namespace GamePlay.PhysicMove
{
    public static class OrbitMovementHelper
    {
        /// <summary>
        /// 将物体的世界坐标转换成新的坐标
        /// </summary>
        /// <param name="originPosition"></param>
        /// <param name="speed"></param>
        /// <param name="radius"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
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
        /// 纯粹坐标计算，传入position，返回基于"塔"的位置
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Vector3 GetPositionOnCircle(Vector3 position, float radius)
        {
            // 计算当前角度
            var angle = Mathf.Atan2(position.z, position.x);

            // 以当前角度和新半径计算新的位置
            var newX = radius * Mathf.Cos(angle);
            var newZ = radius * Mathf.Sin(angle);

            // 距离中心太近时，进行浮点数精度处理
            newX = Mathf.Abs(newX) < 1e-6 ? 0 : newX;
            newZ = Mathf.Abs(newZ) < 1e-6 ? 0 : newZ;

            // 保持原始Y坐标
            var newPosition = new Vector3(newX, position.y, newZ);

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
                Debug.LogError($"[{nameof(OrbitMovementHelper)}]你输入了一个原点位置来求值");
                return default;
            }

            // 通过lookRotation计算四元数旋转，指向XZ方向
            Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
        
            // 获取lookRotation的欧拉角，只适用Y轴
            Vector3 eulerRotation = lookRotation.eulerAngles;

            // 返回一个新的欧拉角度，仅改变Y轴旋转
            return Quaternion.Euler(0, eulerRotation.y, 0);
        }
        
        /// <summary>
        /// 获取经过距离延申后的位置
        /// </summary>
        /// <param name="startPoint">圆心位置</param>
        /// <param name="endPoint">方向位置->比如你要跟随一个物体，那么就是这个物体的位置</param>
        /// <param name="distance">距离这个物体的距离</param>
        /// <returns></returns>
        public static Vector3 GetExtendedPoint(Vector3 startPoint, Vector3 endPoint, float distance)
        {
            // Step 1: Calculate the direction vector from start to end
            Vector3 direction = endPoint - startPoint;
        
            // Step 2: Normalize the direction vector to have a magnitude of 1 (unit vector)
            Vector3 normalizedDirection = direction.normalized;
        
            // Step 3: Scale the direction vector by the desired extension distance
            Vector3 extendedVector = normalizedDirection * distance;
        
            // Step 4: Add the scaled direction vector to the end point to get the extended point
            Vector3 extendedPoint = endPoint + extendedVector;
        
            return extendedPoint;
        }
        
        /// <summary>
        /// 获得新的旋转角度，让物体保持从圆心朝向物体所在位置，忽视Y轴。
        /// </summary>
        /// <param name="position"></param>
        /// <param name="inRoom">true会反向Z指向圆心</param>
        public static Quaternion LookAtZeroPoint(Vector3 position, bool inRoom)
        {
            // 原点（0, 0, 0），但我们忽略高度，只考虑水平平面
            Vector3 targetPosition = new Vector3(0, position.y, 0);

            // 计算到目标位置（原点）的方向向量
            Vector3 direction = targetPosition - position;

            // 忽略Y轴（高度方向）
            direction.y = 0;

            // 确保方向向量不为零
            if (direction.sqrMagnitude > 0.0001f)
            {
                // 如果需要背向目标，反转方向向量
                if (inRoom)
                {
                    direction = -direction;
                }

                // 计算该方向的四元数旋转
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // 更新对象的旋转
                return targetRotation;
            }
            
            return Quaternion.identity;
        }
    }
}