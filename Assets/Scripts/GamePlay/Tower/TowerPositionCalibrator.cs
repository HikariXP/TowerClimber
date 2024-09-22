/*
 * Author: CharSui
 * Created On: 2024.08.25
 * Description: 由塔初始化的时候，将对应的内容调整为对应的位置和旋转
 * 也可以做成仅在Editor的调整性脚本
 */

using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Tower
{
    public class TowerPositionCalibrator : MonoBehaviour
    {
        /// <summary>
        /// 调整位置
        /// </summary>
        /// <param name="towardsCenter">是否室内，影响朝内朝外</param>
        /// <param name="radius">塔的半径</param>
        [Button]
        public void Calibrate(bool towardsCenter, float radius)
        {
            // 获得物体当前位置
            Vector3 currentPosition = transform.position;
        
            // 计算当前物体到圆心的方向向量，仅使用水平面上的 x 和 z
            Vector3 directionToCenter = new Vector3(-currentPosition.x, 0, -currentPosition.z).normalized;
        
            if (!towardsCenter)
            {
                // 如果是反方向，得到与圆心相反的方向
                directionToCenter = -directionToCenter;
            }

            // 计算圆周上新位置
            Vector3 newPositionOnCircle = directionToCenter * radius;
    
            // 更新物体的位置，使其处于圆周上新的位置
            transform.position = newPositionOnCircle;
            
            // 水平面上计算旋转的角度
            // 直接使用directionToCenter来确定物体旋转方向
            Quaternion targetRotation = Quaternion.LookRotation(directionToCenter);
        
            // 将物体旋转到目标角度，但不移动物体位置
            transform.rotation = targetRotation;
        }

        public void Start()
        {
            // Destroy(TowerPositionCalibrator);
        }

        public void OnDestroy()
        {
            Debug.LogError("删掉这个");
        }
    }
}