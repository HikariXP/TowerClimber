/*
 * Author: CharSui
 * Created On: 2024.06.10
 * Description: 平台移动
 */

using GamePlay.PhysicMove;
using UnityEngine;

public class OrbitFollower : MonoBehaviour
{
    public bool OnlyFollowY;
    
    /// <summary>
    /// 是否跟随旋转
    /// </summary>
    public bool FollowRotate = true;
    
    /// <summary>
    /// 距离玩家的距离，而不是距离半径的距离
    /// </summary>
    public float distance = 3f;

    /// <summary>
    /// 塔的水平坐标 : TODO:改为向Helper直接问全局数据
    /// </summary>
    public Vector3 StartPosition = Vector3.zero;

    /// <summary>
    /// 跟随的目标
    /// </summary>
    public Transform FollowTarget;

    /// <summary>
    ///  此坐标不需要处理物理，放在update里面
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void Update()
    {
        var position = FollowTarget.position;

        if (OnlyFollowY)
        {
            var thisPosition = transform.position;
            thisPosition = new Vector3(thisPosition.x, position.y, thisPosition.z);
            transform.position = thisPosition;
            return;
        }

        // 位置跟随
        StartPosition = new Vector3(StartPosition.x, position.y, StartPosition.z);
        transform.position = ObritMovementHelper.GetExtendedPoint(StartPosition, position,distance);

        if (FollowRotate)
        {
            transform.rotation = ObritMovementHelper.GetQuaternionLockToTarget(position,Vector3.zero);
        }
    }
    
}
