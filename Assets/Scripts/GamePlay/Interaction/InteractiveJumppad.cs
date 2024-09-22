/*
 * Author: CharSui
 * Created On: 2024.08.11
 * Description: 触碰型跳板，跳板的原理是覆盖垂直矢量
 */
using System.Collections;
using System.Collections.Generic;
using Module.EventManager;
using UnityEngine;

public class InteractiveJumppad : IInteractiveUnit
{
    [Header("JumpPad Config")] 
    public float jumpPower;
    
    // 可以拓展其他的，比如是给玩家添加一个Buff，持续多少秒的向上多少速度的跳跃。或者是固定速度

    public override void Interact(RigidBodyOrbitMove targetObject)
    {
        var velocity = targetObject.GetVelocity();
        velocity.y = jumpPower;
        targetObject.SetVelocity(velocity);
    }
}
