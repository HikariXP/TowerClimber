/*
 * Author: CharSui
 * Created On: 2024.05.20
 * Description: 用于提供对InputSystem的一层输入监听
 */

using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ISListener : MonoBehaviour
{
    public PlayerInput playerInput;

    public Vector2 movement;

    public Vector2 smoothMovement;

    // public event Action jumpEvent;
    public bool jump;
    public void MovementInputListen(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public void JumpInputListen(InputAction.CallbackContext context)
    {
        // jump = context.started;
        jump = context.performed;
        
        // 由于检查在fixupdate，所以大概率没在fixupdate那帧跳跃会失效，一定需要改正。改成在update里面，将其缓存一小会去等待fixedUpdate吃掉这个指令。
        if(context.started)Debug.Log("Jump");
    }

    public void Update()
    {
        smoothMovement = Vector2.Lerp(smoothMovement, movement, 0.1f);
    }
}
