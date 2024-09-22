/*
 * Author: CharSui
 * Created On: 2024.06.15
 * Description: 用于取代所有CharacterController的移动内容
 * TODO：可以后期做一个通用的移动方案。方便做AI
 */

using System;
using GamePlay.PhysicMove;
using Module.EventManager;
using Sirenix.OdinInspector;
using UnityEngine;

public class RigidBodyOrbitMove : MonoBehaviour, IPause
{
    private EventManager _battleEventManager;
    
    private Rigidbody _rigidBody;

    [SerializeField]
    private Transform _characterTransform;
    
    /// <summary>
    /// 取消激活就应用任何物理，交由其他组件控制
    /// </summary>
    [Header("Player"),ReadOnly]
    public bool isActive = true;
    
    [ReadOnly]
    public bool listenInput = true;

    public bool isInRoom = false;
    
    /// <summary>
    /// X轴上的输入。
    /// </summary>
    public float moveInput;

    public bool jumpInput;
    
    /// <summary>
    /// 塔的半径
    /// </summary>
    public float OrbitRadius;
    
    public float JumpForce = 5f;
    
    /// <summary>
    /// 角色朝向旋转速度
    /// </summary>
    public float directionRotationSpeed = 5f;

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

    /// <summary>
    /// TODO:改为曲线控制
    /// </summary>
    public float Gravity = 9.8f;

    public AnimationCurve gravityCurve;

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
    private Vector2 _moveVelocity;

    /// <summary>
    /// 环境矢量，通常由传送带，或者风场造成
    /// 通常到达区域后赋予常量
    /// TODO:改成可以接收多个量，自动结算最终影响量
    /// </summary>
    private Vector2 _environmentVelocity;
    
    /// <summary>
    /// 最终矢量的最大绝对值
    /// </summary>
    public float HorizionalVelocityMax = 10f;
    
    /// <summary>
    /// 最终矢量的最大绝对值
    /// </summary>
    public float VerticalVelocityMax = 5f;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _battleEventManager = EventHelper.GetEventManager(EventManagerType.BattleEventManager);
        
        // 测试由动画曲线获取重力的方法，如果超Key则获取最近的值
        // Debug.Log(gravityCurve.Evaluate(-1f));
    }

    public void ControlledUpdate(float deltaTime)
    {
        if(!isActive) return;
        if(deltaTime < 0.0001f) return;
        if (OrbitRadius <= 0) return;
        // 这里不处理，就没办法做到平台移动。
        if (listenInput && isGround) _moveVelocity.x = moveInput;

        if (Math.Abs(moveInput) > 0.1f)
        {
            if (moveInput > 0)
            {
                if (isInRoom)
                {
                    
                }
            }
        }

        // ========================================= 前处理
        
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
        // 改为重力曲线
        // GravityCurveCheck(fixedDeltaTime, _moveVelocity.y);
        
        JumpCheck(); 
        CheckCeilingCollision();

        // 如果在室内，水平矢量反转
        // TODO:做到获取获取水平移动上？感觉不太好
        var finalVelocity = _environmentVelocity + _moveVelocity;

        Vector2LimitCheck(ref finalVelocity, HorizionalVelocityMax, VerticalVelocityMax);
        
        
        // 角色旋转代码
        // 移动输入大于0.1的时候旋转角色
        if (Mathf.Abs(_moveVelocity.x) > 0.1f)
        {
            Debug.Log("Running");
            var v2 = _rigidBody.velocity;
            if (v2 != Vector3.zero)
            {
                Vector3 horizontalVelocity = new Vector3(v2.x, 0, v2.z);
                Vector3 direction = horizontalVelocity.normalized;
                Quaternion q = Quaternion.LookRotation(direction);
                _characterTransform.rotation = q;
            }
        }
        
        if (isInRoom)
        {
            var reverseXVelocity = new Vector2(-finalVelocity.x, finalVelocity.y);
            finalVelocity = reverseXVelocity;
        }

        

        Vector3 position = _rigidBody.position;


        // ========================================= 速度计算
        // 计算水平移动的新位置
        Vector3 originHorizontalMovement = OrbitMovementHelper.GetHorizontalMovement(position, finalVelocity.x, OrbitRadius, fixedDeltaTime);

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
            if (_moveVelocity.y > 0)
            {
                _moveVelocity.y = 0;
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
        _forceTranslatePosition = OrbitMovementHelper.GetPositionOnCircle(_forceTranslatePosition, OrbitRadius);
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
            _moveVelocity.y = 0;
        }

        

        //TODO:需要添加对于斜面的检查
    }

    /// <summary>
    /// 固定重力计算
    /// </summary>
    void GravityCheck(float deltaTime)
    {
        if (isGround) return;

        // 叠加垂直矢量，重力是垂直向下的，所以需要减
        _moveVelocity.y -= Gravity * deltaTime;
    }
    
    /// <summary>
    /// 重力曲线计算
    /// </summary>
    /// <param name="deltaTime"></param>
    void GravityCurveCheck(float deltaTime, float verticalVelocity)
    {
        if (isGround) return;

        var gravityValue = gravityCurve.Evaluate(verticalVelocity);
        
        // 叠加垂直矢量，重力是垂直向下的，所以需要减
        _moveVelocity.y -= gravityValue * deltaTime;
    }

    void JumpCheck()
    {
        // 空中无法起跳
        if(!isGround)return;
        
        // 没按跳跃无法起跳
        if(!jumpInput)return;

        var velocity = _rigidBody.velocity;
        velocity = new Vector3(velocity.x, JumpForce, velocity.z);
        
        _rigidBody.velocity = velocity;
    }

    void Vector2LimitCheck(ref Vector2 vectorValue, float xMax, float yMax)
    {
        // 取最大值的绝对值
        var maxXValueAbs = Mathf.Abs(xMax);
        var maxYValueAbs = Mathf.Abs(yMax);

        // 如果 x 分量的绝对值超过最大值，则限制它
        if (Mathf.Abs(vectorValue.x) > maxXValueAbs)
        {
            vectorValue.x = Mathf.Sign(vectorValue.x) * maxXValueAbs;
        }

        // 如果 y 分量的绝对值超过最大值，则限制它
        if (Mathf.Abs(vectorValue.y) > maxYValueAbs)
        {
            vectorValue.y = Mathf.Sign(vectorValue.y) * maxYValueAbs;
        }
    }
    
    #endregion Physic - 物理判断
    
    
    /// <summary>
    /// 设置移动控制器的基础信息
    /// </summary>
    /// <param name="radius"></param>
    public void SetupObritInfo(float radius)
    {
        OrbitRadius = radius;
    }
    
    public void PlayerMove(float vectorX)
    {
        if(!listenInput) return;

        moveInput = vectorX;
    }

    public Vector2 GetVelocity()
    {
        return new Vector2(_moveVelocity.x, _moveVelocity.y);
    }

    /// <summary>
    /// 强制应用新的的矢量
    /// </summary>
    public void SetVelocity(Vector2 v2)
    {
        _moveVelocity = v2;
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
        _moveVelocity = vector;
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
