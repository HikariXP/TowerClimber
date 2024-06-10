/*
 * Author: CharSui
 * Created On: 2024.05.26
 * Description: 最优解是通过rigidbody，自己处理所有的惯性矢量问题
 * 这个和RigidBodyObritMove几乎一致，只是部分API使用上和RigidBody有出入，应当随时同步两者的实现。
 *
 * TODO:可能需要改命令式输入。
 */

using System;
using System.Collections;
using System.Collections.Generic;
using GamePlay.PhysicMove;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerObritMove : MonoBehaviour
{ 
    /// <summary>
    /// X轴上的输入。
    /// </summary>
    public float moveInput;

    public bool jumpInput;

    private Transform _transform;
    
    private CharacterController _characterController;

    public bool IsGround => _characterController.isGrounded;

    /// <summary>
    /// 塔的半径
    /// </summary>
    public float _ObritRadius;

    public float Speed = 7.8f;

    public float JumpForce = 10f;

    public bool listenInput = true;
    
    [Header("着地信息")]
    public float Gravity;

    public float GroundOffset;

    public float GroundRadius;

    /// <summary>
    /// 水平矢量
    /// </summary>
    [Header("模拟矢量信息")]
    public float _VectorX;
    
    /// <summary>
    /// 绝对值
    /// </summary>
    public float HorizionalVelocityMax = 1f;
    
    /// <summary>
    /// 垂直矢量
    /// </summary>
    public float _VectorY;

    /// <summary>
    /// 绝对值
    /// </summary>
    public float VerticalVelocityMax = 1f;
    
    [Header("触顶处理")]
    public LayerMask ceilingMask;
    public float ceilingCheckDistance = 0.1f;
    

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _transform = transform;
    }

    private void Update()
    {
        // 输入类型的Update检测
        // JumpCheck();
    }

    private void FixedUpdate()
    {
        // 输入检测
        // 测试用
        if (listenInput && IsGround) _VectorX = moveInput * Speed;

        // 矢量检查
        GravityCheck();
        // 头顶检测
        CheckCeilingCollision();
        
        // 最大矢量约束
        _VectorX = Math.Clamp(_VectorX, -HorizionalVelocityMax, HorizionalVelocityMax);
        _VectorY = Math.Clamp(_VectorY, -HorizionalVelocityMax, HorizionalVelocityMax);
        
        Vector3 tempMovement = ObritMovementHelper.GetHorizontalMovement(_transform.position, _VectorX, _ObritRadius, Time.deltaTime);
        float tempVertical = GetVerticalMovement();
        Vector3 resultMovement = new Vector3(tempMovement.x, tempVertical, tempMovement.z);

        //ChacaterController是直接基于自己去移动，而不是刚体那种可以直接移动去某个地方
        Vector3 newLocalPosition = transform.InverseTransformPoint(resultMovement);
        _characterController.Move(newLocalPosition);
    }

    /// <summary>
    /// 设置移动控制器的基础信息
    /// 塔的半径几乎不会随时变动，但是速度会。
    /// </summary>
    /// <param name="radius"></param>
    public void SetupObritInfo(float radius)
    {
        _ObritRadius = radius;
    }

    /// <summary>
    /// 强制应用新的的矢量
    /// </summary>
    public void ForceApplyNewVelocity(Vector2 v2)
    {
        _VectorX = v2.x;
        _VectorY = v2.y;
    }

    /// <summary>
    /// 重力计算
    /// </summary>
    void GravityCheck()
    {
        // 着地垂直矢量归 0
        if (IsGround && _VectorY < 0f)
        {
            _VectorY = -0.1f;
            return;
        }

        // 叠加垂直矢量，重力是垂直向下的，所以需要减
        _VectorY -= Gravity * Time.deltaTime;
    }

    float GetVerticalMovement()
    {
        // 现在这套方案，会导致跳跃的时候顶头的话，不会正常削减向上的矢量
        return transform.position.y + _VectorY * Time.deltaTime;
    }
    
    /// <summary>
    /// 检查触顶矢量处理
    /// TODO:改成区域型的检测，而不是单独头顶一根射线处理
    /// </summary>
    private void CheckCeilingCollision()
    {
        // Perform a raycast upwards from the character
        if (Physics.Raycast(transform.position, Vector3.up, out var hit, _characterController.height / 2 + ceilingCheckDistance, ceilingMask))
        {
            if (_VectorY > 0)
            {
                _VectorY = 0;
            }
        }
    }

    #region Debug - Gizmos

    private void OnDrawGizmosSelected()
    {

    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            DrawOnPlaying();
        }
    }

    private void DrawOnPlaying()
    {
        // 测试，代码需要包含在OnDrawGizoms或者OnDrawGizmosSelected
        var position = transform.position;
        Vector3 spherePosition = new Vector3(position.x, position.y - GroundOffset,
            position.z);
        Gizmos.DrawSphere(spherePosition, GroundRadius);
        
        Gizmos.DrawRay(transform.position,new Vector3(0,_characterController.height / 2 + ceilingCheckDistance,0));
    }

    #endregion
    

    #region 实验代码
    
    void MoveCircle(float speed, Transform transform, float radius, float deltaTime)
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
    
    private void GroundedCheck()
    {
        bool Grounded;
        float GroundedRadius = 1f;
        LayerMask GroundLayers = LayerMask.GetMask("123");
        float GroundedOffset = 1f;
        
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
    }
    
    #endregion
    
}
