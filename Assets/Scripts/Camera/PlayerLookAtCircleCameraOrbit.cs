/*
 * Author: CharSui
 * Created On: 2024.08.08
 * Description: 忽视高度，一直看着圆心
 * 没有半径设置，直接是一个基于玩家，看着圆心的点。
 * 看着圆心这个操作是用来给VirtualCamera去定位的。如果VirtualCamera看的目标没有旋转，那么就会基于旋转后的视角去处理。
 */

using GamePlay.PhysicMove;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerLookAtCircleCameraOrbit : MonoBehaviour
{
    // Update is called once per frame
    public Transform followObject;

    /// <summary>
    /// 反向
    /// </summary>
    public bool isInRoom;
    
    void Update()
    {
        if(followObject==null) return;

        var followObjectPosition = followObject.position;

        transform.position = followObjectPosition;
        
        LookAtZeroPoint(followObjectPosition, isInRoom);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="inRoom">true会反向Z指向圆心</param>
    private void LookAtZeroPoint(Vector3 position, bool inRoom)
    {
        var rotation = OrbitMovementHelper.LookAtZeroPoint(position, inRoom);
        transform.rotation = rotation;
    }

#if UNITY_EDITOR
    [Button]
    public void FixPosition()
    {
        Update();
    }
    #endif
}
