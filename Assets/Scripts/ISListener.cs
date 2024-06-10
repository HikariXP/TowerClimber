/*
 * Author: CharSui
 * Created On: 2024.05.20
 * Description: 用于提供对InputSystem的一层输入监听
 */

using System;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ISListener : MonoBehaviour
{
    [Sirenix.OdinInspector.ReadOnly]
    private Vector2 movement;

    [Sirenix.OdinInspector.ReadOnly]
    public Vector2 smoothMovement;

    public event Action jumpEvent;
    public void MovementInputListen(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public void JumpInputListen(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            Debug.Log($"jump");
            jumpEvent?.Invoke();
        }
    }

    public void Update()
    {
        smoothMovement = Vector2.Lerp(smoothMovement, movement, 0.1f);
    }
}
