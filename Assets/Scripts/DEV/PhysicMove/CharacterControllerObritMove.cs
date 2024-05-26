/*
 * Author: CharSui
 * Created On: 2024.05.26
 * Description: 最优解是通过rigidbody，自己处理所有的惯性矢量问题
 * 这个和RigidBodyObritMove几乎一致，只是部分API使用上和RigidBody有出入，应当随时同步两者的实现。
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerObritMove : MonoBehaviour
{ 
    /// <summary>
    /// X轴上的输入。
    /// </summary>
    public float moveInput;

    public float jumpInput;

    private CharacterController _CharacterController;

    /// <summary>
    /// 塔的半径
    /// </summary>
    public float _ObritRadius;
    
    [Header("着地信息")] 
    public bool isGround;

    public float Gravity;

    public float GroundOffset;

    public float GroundRadius;
    
    public LayerMask GroundLayerMask;


    
    /// <summary>
    /// 水平矢量
    /// </summary>
    private float _VectorX;
    
    /// <summary>
    /// 绝对值
    /// </summary>
    public float HorizionalVelocityMax = 1f;
    
    /// <summary>
    /// 垂直矢量
    /// </summary>
    private float _VectorY;

    /// <summary>
    /// 绝对值
    /// </summary>
    public float VerticalVelocityMax = 1f;
    

    private void Awake()
    {
        _CharacterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        // TODO：位置纠正：解决角色的脚插了进地里的问题
        

        // 输入检测
        // 测试用
        _VectorX = moveInput;
        
        // 矢量检查
        GroundCheck(GroundRadius,GroundOffset);
        GravityCheck();
        JumpCheck();
        
        // 矢量应用
        VectorLimitCheck(ref _VectorX, HorizionalVelocityMax);
        VectorLimitCheck(ref _VectorY, VerticalVelocityMax);
        
        Vector3 tempMovement = GetHorizontalMovement(_VectorX, _ObritRadius, Time.deltaTime);
        float tempVertical = GetVerticalMovement();
        Vector3 resultMovement = new Vector3(tempMovement.x, tempVertical, tempMovement.z);
        
        //ChacaterController是直接基于自己去移动，而不是刚体那种可以直接移动去某个地方
        Vector3 newLocalPosition = transform.InverseTransformPoint(resultMovement);
        
        // _RigidBody.MovePosition(resultMovement);
        // _CharacterController.Move(resultMovement);
        _CharacterController.Move(newLocalPosition);
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
    /// 着地检查
    /// </summary>
    /// <param name="groundedRadius"></param>
    /// <param name="groundedOffset"></param>
    void GroundCheck(float groundedRadius, float groundedOffset)
    {
        // var position = transform.position;
        // Vector3 spherePosition = new Vector3(position.x, position.y - groundedOffset,
        //     position.z);
        //
        // isGround = Physics.CheckSphere(spherePosition, groundedRadius, GroundLayerMask,
        //     QueryTriggerInteraction.Ignore);
        //

        isGround = _CharacterController.isGrounded;

        //TODO:需要添加对于斜面的检查
    }

    /// <summary>
    /// 重力计算
    /// </summary>
    void GravityCheck()
    {
        // 着地垂直矢量归 0
        if (isGround)
        {
            _VectorY = 0f;
            return;
        }

        // 叠加垂直矢量，重力是垂直向下的，所以需要减
        _VectorY -= Gravity * Time.deltaTime;
    }

    void JumpCheck()
    {
        // 空中无法起跳
        if(!isGround)return;
        
        // 没按跳跃无法起跳
        if(jumpInput == 0)return;

        _VectorY += jumpInput;
    }

    /// <summary>
    /// 保底处理
    /// </summary>
    void VectorLimitCheck(ref float vectorValue, float maxValue)
    {
        var maxValueAbs = Mathf.Abs(maxValue);
        
        if (maxValueAbs < vectorValue)
        {
            vectorValue = maxValueAbs;
        }

        if (vectorValue < -maxValueAbs)
        {
            vectorValue = -maxValueAbs;
        }
    }

    /// <summary>
    /// 水平上的移动
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="radius"></param>
    /// <param name="deltaTime"></param>
    Vector3 GetHorizontalMovement(float speed, float radius, float deltaTime)
    {
        // 如果速度为0，直接返回当前位置
        if (speed == 0)
        {
            return transform.position;
        }

        // 计算弧长
        float distanceToMove = speed * deltaTime;

        // 计算弧度
        float angleToMove = distanceToMove / radius;

        // 计算当前角度
        Vector3 currentPosition = transform.position;
        float currentAngle = Mathf.Atan2(currentPosition.z, currentPosition.x);

        // 计算新的角度
        float newAngle = currentAngle + angleToMove;

        // 以新的角度计算新的位置
        float newX = radius * Mathf.Cos(newAngle);
        float newZ = radius * Mathf.Sin(newAngle);

        // 距离中心太近时，进行浮点数精度处理
        newX = Mathf.Abs(newX) < 1e-6 ? 0 : newX;
        newZ = Mathf.Abs(newZ) < 1e-6 ? 0 : newZ;

        Vector3 newPosition = new Vector3(newX, currentPosition.y, newZ);

        return newPosition;
    }

    float GetVerticalMovement()
    {
        return transform.position.y + _VectorY * Time.deltaTime;
    }

    #region Debug

    private void OnDrawGizmosSelected()
    {
        // 测试，代码需要包含在OnDrawGizoms或者OnDrawGizmosSelected
        var position = transform.position;
        Vector3 spherePosition = new Vector3(position.x, position.y - GroundOffset,
            position.z);
        Gizmos.DrawSphere(spherePosition, GroundRadius);
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
