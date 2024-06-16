/*
 * Author: CharSui
 * Created On: 2024.06.15
 * Description: 用于取代所有CharacterController的移动内容
 * TODO：可以后期做一个通用的移动方案。方便做AI
 */

using System;
using System.Collections;
using System.Collections.Generic;
using GamePlay.PhysicMove;
using Module.EventManager;
using Sirenix.OdinInspector;
using UnityEngine;

public class RigidBodyObritMove : MonoBehaviour
{
    // Components
    private Transform _transform;

    private EventManager _battleEventManager;
    
    private Rigidbody _rigidBody;
    
    [Header("Player")]
    public bool canInput;
    /// <summary>
    /// X轴上的输入。
    /// </summary>
    public float moveInput;

    public bool jumpInput;

    

    /// <summary>
    /// 塔的半径
    /// </summary>
    public float ObritRadius;
    
    public float JumpForce = 5f;

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
    public bool checkGounded = true;
    
    public bool isGround;

    public float Gravity = 9.8f;

    public float GroundOffset;

    public float GroundRadius;
    
    public LayerMask GroundLayerMask;
    
    //public float Gravity; 暂时用不上，当前方案使用RigidBody的重力系统
    
    [Header("触顶处理")]
    public LayerMask ceilingMask;
    public float ceilingCheckDistance = 0.1f;

    // 强制移动相关
    private bool _forceTranslateNextFixedUpdate = false;
    private Vector3 _forceTranslatePosition = Vector3.zero;
    
    [ShowInInspector]
    private Vector2 _velocity;

    /// <summary>
    /// 环境矢量，通常由传送带，或者风场造成
    /// 通常到达区域后赋予常量
    /// TODO:改成可以接收多个量，自动结算最终影响量
    /// </summary>
    private Vector2 _environmentVelocity;
    
    /// <summary>
    /// 绝对值
    /// </summary>
    public float HorizionalVelocityMax = 1f;
    
    /// <summary>
    /// 绝对值
    /// </summary>
    public float VerticalVelocityMax = 1f;

    public void Start()
    {
        // Time.timeScale = 0.3f;
    }

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
        
        _battleEventManager = EventHelper.GetEventManager(EventManagerType.BattleEventManager);
    }

    private void FixedUpdate()
    {
        // ========================================= 前处理
        // 这里不处理，就没办法做到平台移动。
        if (canInput && isGround) _velocity.x = moveInput;
        
        // 强制跳转
        if (_forceTranslateNextFixedUpdate)
        {
            ForceTranslate();
            _forceTranslateNextFixedUpdate = false;
            return;
        }
        
        float fixedDeltaTime = Time.fixedDeltaTime;

        // 着地检测。跟置于脚下的碰撞体不一样，简化后的碰撞检测
        if(checkGounded) GroundCheck(GroundRadius,GroundOffset);

        // 需要自己计算重力
        GravityCheck(fixedDeltaTime);
        JumpCheck();
        CheckCeilingCollision();

        var finalVelocity = _environmentVelocity + _velocity;

        Vector3 position = _rigidBody.position;
        

        // ========================================= 速度计算
        // 计算水平移动的新位置
        Vector3 originHorizontalMovement = ObritMovementHelper.GetHorizontalMovement(position, finalVelocity.x, ObritRadius, fixedDeltaTime);

        // 计算新的Y轴位置
        float y = position.y + finalVelocity.y * fixedDeltaTime;

        // 组合新的位置
        Vector3 newPosition = new Vector3(originHorizontalMovement.x, y, originHorizontalMovement.z);

        // 计算速度向量
        Vector3 velocity = (newPosition - position) / fixedDeltaTime;

        // 将速度从世界坐标转换为局部坐标
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        // 应用速度到刚体
        _rigidBody.velocity = localVelocity;

        // ========================================= 后处理
        // 纠正碰墙矢量
        WallCheckAndAdjustVelocity();
        
        // 在平台上跳跃会失去势能。
        // _environmentVelocity = Vector2.zero;
    }

    #region Physic - 物理判断

    /// <summary>
    /// 检查触顶矢量处理
    /// TODO:改成区域型的检测，而不是单独头顶一根射线处理
    /// </summary>
    private void CheckCeilingCollision()
    {
        // Perform a raycast upwards from the character
        if (Physics.Raycast(transform.position, Vector3.up, out var hit, 1 + ceilingCheckDistance, ceilingMask))
        {
            if (_velocity.y > 0)
            {
                _velocity.y = 0;
            }
        }
    }
    
    void WallCheckAndAdjustVelocity()
    {
        var velocity = _rigidBody.velocity;
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

            Vector3 currentVelocity = _rigidBody.velocity;

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
                _rigidBody.velocity = velocityAlongWall;
            }
        }
        else
        {
            if (isWallSliding) isWallSliding = false;
        }
    }
    
    /// <summary>
    /// fixdeUpdate里面运行
    /// </summary>
    private void ForceTranslate()
    {
        // 进行一次刷新
        _forceTranslatePosition = ObritMovementHelper.GetPositionOnCircle(_forceTranslatePosition, ObritRadius);
        _rigidBody.MovePosition(_forceTranslatePosition);
    }

    /// <summary>
    /// 着地检查
    /// </summary>
    /// <param name="groundedRadius"></param>
    /// <param name="groundedOffset"></param>
    void GroundCheck(float groundedRadius, float groundedOffset)
    {
        var groundedLastFrame = isGround;
        
        var position = transform.position;
        Vector3 spherePosition = new Vector3(position.x, position.y - groundedOffset,
            position.z);
        
        isGround = Physics.CheckSphere(spherePosition, groundedRadius, GroundLayerMask,
            QueryTriggerInteraction.Ignore);

        if (!groundedLastFrame && isGround)
        {
            _battleEventManager.TryGetNoArgEvent(BattleEventDefine.PLAYER_GROUNDED_START).Notify();
            _environmentVelocity = Vector2.zero;
            _velocity.y = 0;
        }

        

        //TODO:需要添加对于斜面的检查
    }

    /// <summary>
    /// 重力计算
    /// </summary>
    void GravityCheck(float deltaTime)
    {
        // 着地垂直矢量归 0
        if (isGround)
        {
            // _velocityY = 0f;
            return;
        }

        // 叠加垂直矢量，重力是垂直向下的，所以需要减
        _velocity.y -= Gravity * deltaTime;
    }

    void JumpCheck()
    {
        // 空中无法起跳
        if(!isGround)return;
        
        // 没按跳跃无法起跳
        if(!jumpInput)return;

        // 更换了新的实现方案，操作上只接管xz，y轴直接由rigidbody管控
        // _VectorY += _JumpForce;

        var velocity = _rigidBody.velocity;
        velocity = new Vector3(velocity.x, JumpForce, velocity.z);
        
        _rigidBody.velocity = velocity;
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
    
    #endregion Physic - 物理判断
    
    
    /// <summary>
    /// 设置移动控制器的基础信息
    /// </summary>
    /// <param name="radius"></param>
    public void SetupObritInfo(float radius)
    {
        ObritRadius = radius;
    }
    
    public void PlayerMove(float vectorX)
    {
        if(!canInput) return;

        moveInput = vectorX;
    }

    public Vector2 GetVelocity()
    {
        return new Vector2(_velocity.x, _velocity.y);
    }

    /// <summary>
    /// 强制应用新的的矢量
    /// </summary>
    public void SetVelocity(Vector2 v2)
    {
        _velocity = v2;
    }

    public void SetEnvironmentVelocity(Vector2 v2)
    {
        _environmentVelocity = v2;
    }

    /// <summary>
    /// 只传送，不影响矢量
    /// </summary>
    /// <param name="position"></param>
    public void ForceTranslateTo(Vector3 position)
    {
        _forceTranslateNextFixedUpdate = true;
        _forceTranslatePosition = position;
    }

    /// <summary>
    /// 传送且修改矢量
    /// </summary>
    /// <param name="position"></param>
    /// <param name="vector"></param>
    public void ForceTranslateTo(Vector3 position, Vector2 vector)
    {
        _forceTranslateNextFixedUpdate = true;
        _forceTranslatePosition = position;
        _velocity = vector;
    }


    

#if UNITY_EDITOR

    [Button]
    public void EditorSetVelocity(Vector2 velocity)
    {
        SetVelocity(velocity);
    }

#endif

    #region Debug

    private void OnDrawGizmos()
    {
        // 着地检测
        var position = transform.position;
        Vector3 spherePosition = new Vector3(position.x, position.y - GroundOffset,
            position.z);
        Gizmos.DrawSphere(spherePosition, GroundRadius);
    }

    private void OnDrawGizmosSelected()
    {
        // -Runtime Debug- //
        
        if(!Application.isPlaying) return;
        
        // 测试，代码需要包含在OnDrawGizoms或者OnDrawGizmosSelected

        
        // 矢量部分
        var velocity = _rigidBody.velocity;
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
            // Debug.Log("Radial Direction: " + radialDirection);
            // Debug.Log("Tangent Direction: " + tangentDirection);

            // 绘制切线方向的矢量箭头
            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, position + tangentDirection * 6);
            Gizmos.DrawSphere(position + tangentDirection * 6, 0.1f); // 可以用一个小球表示头部
        }
    }

    #endregion
}
