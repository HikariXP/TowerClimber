/*
 * Author: CharSui
 * Created On: 2024.05.26
 * Description: 最优解是通过rigidbody，自己处理所有的惯性矢量问题
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidBodyObritMove : MonoBehaviour
{
    /// <summary>
    /// X轴上的输入。
    /// </summary>
    public float moveInput;

    public bool jumpInput;

    private Rigidbody _RigidBody;

    /// <summary>
    /// 塔的半径
    /// </summary>
    public float _ObritRadius;
    
    public float _JumpForce = 5f;

    /// <summary>
    /// 碰墙检测距离
    /// </summary>
    [Header("碰墙检查")]
    public float wallDetectionDistance = 2f;

    public float wallCheckOffset = 0.5f;
    
    /// <summary>
    /// 是否正在滑墙
    /// </summary>
    public bool isWallSliding = false;
    
    

    [Header("着地信息")] 
    public bool isGround;

    public float GroundOffset;

    public float GroundRadius;
    
    public LayerMask GroundLayerMask;
    
    //public float Gravity; 暂时用不上，当前方案使用RigidBody的重力系统

    
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

    public void Start()
    {
        Time.timeScale = 0.3f;
    }

    private void Awake()
    {
        _RigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // 输入检测
        // 测试用
        _VectorX = moveInput;
        
        // 着地检测。跟置于脚下的碰撞体不一样，简化后的碰撞检测
        GroundCheck(GroundRadius,GroundOffset);
        
        // 使用RigidBody的重力，不再需要自己计算重力
        // GravityCheck();
        JumpCheck();
        
        Vector3 originHorizontalMovement = GetHorizontalMovement(_VectorX, _ObritRadius, 1);

        var inverseHorizontalMovement = transform.InverseTransformPoint(originHorizontalMovement);

        var result = new Vector3(inverseHorizontalMovement.x, _RigidBody.velocity.y, inverseHorizontalMovement.z);
        
        _RigidBody.velocity = result;

        // 纠正碰墙矢量
        WallCheckAndAdjustVelocity();
    }
    

    void WallCheckAndAdjustVelocity()
    {
        var velocity = _RigidBody.velocity;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        Vector3 direction = horizontalVelocity.normalized;

        var startPosition =
            new Vector3(transform.position.x, transform.position.y + wallCheckOffset, transform.position.z);
        // 射线检测墙壁
        bool wallDetected = Physics.Raycast(startPosition, direction, wallDetectionDistance);

        if (wallDetected)
        {
            if (!isWallSliding)
            {
                isWallSliding = true;
            }

            Vector3 currentVelocity = _RigidBody.velocity;

            // 获取碰撞法线
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, wallDetectionDistance))
            {
                Vector3 wallNormal = hit.normal;

                // 投影速度到墙平面上
                Vector3 velocityAlongWall = Vector3.ProjectOnPlane(currentVelocity, wallNormal);

                // 保持垂直方向的速度
                velocityAlongWall.y = currentVelocity.y;

                // 应用新的速度
                _RigidBody.velocity = velocityAlongWall;
            }
        }
        else
        {
            if (isWallSliding) isWallSliding = false;
        }
    }
    
    

    /// <summary>
    /// 设置移动控制器的基础信息
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
        var position = transform.position;
        Vector3 spherePosition = new Vector3(position.x, position.y - groundedOffset,
            position.z);
        
        isGround = Physics.CheckSphere(spherePosition, groundedRadius, GroundLayerMask,
            QueryTriggerInteraction.Ignore);
        

        
        //TODO:需要添加对于斜面的检查
    }

    // /// <summary>
    // /// 重力计算
    // /// </summary>
    // void GravityCheck()
    // {
        // 着地垂直矢量归 0
        // if (isGround)
        // {
        //     _VectorY = 0f;
        //     return;
        // }

        // 叠加垂直矢量，重力是垂直向下的，所以需要减
        // _VectorY -= Gravity * Time.deltaTime;
    // }

    void JumpCheck()
    {
        // 空中无法起跳
        if(!isGround)return;
        
        // 没按跳跃无法起跳
        if(!jumpInput)return;

        // 更换了新的实现方案，操作上只接管xz，y轴直接由rigidbody管控
        // _VectorY += _JumpForce;

        var velocity = _RigidBody.velocity;
        velocity = new Vector3(velocity.x, _JumpForce, velocity.z);
        
        _RigidBody.velocity = velocity;
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

        // 计算新的角度, 逆时针
        float newAngle = currentAngle - angleToMove;

        // 以新的角度计算新的位置
        float newX = radius * Mathf.Cos(newAngle);
        float newZ = radius * Mathf.Sin(newAngle);
        
        // 距离中心太近时，进行浮点数精度处理
        newX = Mathf.Abs(newX) < 1e-6 ? 0 : newX;
        newZ = Mathf.Abs(newZ) < 1e-6 ? 0 : newZ;
        
        Vector3 newPosition = new Vector3(newX, currentPosition.y, newZ);
        return newPosition;

        // 更新Transform的位置
        // transform.position = newPosition;
    }

    float GetVerticalMovement()
    {
        // return transform.position.y + _VectorY * Time.deltaTime;
        return transform.position.y + _VectorY;
    }

    #region Debug

    private void OnDrawGizmosSelected()
    {
        // -Runtime Debug- //
        
        if(!Application.isPlaying) return;
        
        // 测试，代码需要包含在OnDrawGizoms或者OnDrawGizmosSelected
        var position = transform.position;
        Vector3 spherePosition = new Vector3(position.x, position.y - GroundOffset,
            position.z);
        Gizmos.DrawSphere(spherePosition, GroundRadius);
        
        // 矢量部分
        var velocity = _RigidBody.velocity;
        if (velocity != Vector3.zero)
        {
            Gizmos.color = Color.blue;
            Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
            Vector3 direction = horizontalVelocity.normalized;
            // Gizmos.DrawRay(transform.position, direction * wallDetectionDistance);
            Gizmos.DrawRay(transform.position, direction * 5);
        }
        
        // 墙检测
        Gizmos.color = Color.yellow;
        var startPosition =
            new Vector3(transform.position.x, transform.position.y + wallCheckOffset, transform.position.z);
        Gizmos.DrawSphere(startPosition, 0.5f);


        TangentLineDisplay();
    }

    private void TangentLineDisplay()
    {
        //======= 切线方向
        Vector3 position = transform.position;
        
        // 绘制代表圆的线
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(position, 10);

        // 计算当前点的切线方向
        Vector3 radialDirection = new Vector3(position.x, 0, position.z).normalized;

        // 切线方向是与径向方向垂直的方向
        Vector3 tangentDirection;
        if (radialDirection != Vector3.zero)
        {
            tangentDirection = Vector3.Cross(radialDirection, Vector3.up).normalized; // 因为圆在XZ平面

            // Debug: Log the directions
            Debug.Log("Radial Direction: " + radialDirection);
            Debug.Log("Tangent Direction: " + tangentDirection);

            // 绘制切线方向的矢量箭头
            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, position + tangentDirection * 6);
            Gizmos.DrawSphere(position + tangentDirection * 6, 0.1f); // 可以用一个小球表示头部
        }
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
