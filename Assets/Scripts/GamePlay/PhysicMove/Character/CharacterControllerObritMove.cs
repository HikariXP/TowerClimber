/*
 * Author: CharSui
 * Created On: 2024.05.26
 * Description: 最优解是通过rigidbody，自己处理所有的惯性矢量问题
 * 这个和RigidBodyObritMove几乎一致，只是部分API使用上和RigidBody有出入，应当随时同步两者的实现。
 *
 * TODO:可能需要改命令式输入。
 */

using System;
using GamePlay.PhysicMove;
using Module.EventManager;
using Sirenix.OdinInspector;
using UnityEngine;

[Obsolete("之后全部内容使用RigidBody处理")]
[RequireComponent(typeof(CharacterController))]
public class CharacterControllerObritMove : MonoBehaviour
{ 
    // TODO:将输入抽出，这个CCOM只处理输入后的移动内容。
    
    /// <summary>
    /// X轴上的输入。
    /// </summary>
    [ShowInInspector]
    private float playerMoveInput;

    /// <summary>
    /// 来自环境的输入
    /// </summary>
    private float environmentMoveInput;

    private Transform _transform;
    
    private CharacterController _characterController;

    private EventManager _battleEventManager;
    
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

    public bool IsGround { get; private set; }

    /// <summary>
    /// 水平矢量
    /// </summary>
    [Header("模拟矢量信息")]
    public float _VectorX;
    
    /// <summary>
    /// 绝对值
    /// </summary>
    public float HorizionalVelocityMax = 10f;
    
    /// <summary>
    /// 垂直矢量
    /// </summary>
    public float _VectorY;

    /// <summary>
    /// 绝对值
    /// </summary>
    public float VerticalVelocityMax = 10f;
    
    [Header("触顶处理")]
    public LayerMask ceilingMask;
    public float ceilingCheckDistance = 0.1f;

    // 强制移动相关
    private bool _forceTranslateNextFixedUpdate = false;
    private Vector3 _forceTranslatePosition = Vector3.zero;

    [Header("Debug")] 
    private Vector3 _moveDirection;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _transform = transform;

        _battleEventManager = EventHelper.GetEventManager(EventManagerType.BattleEventManager);
    }

    private void FixedUpdate()
    {
        if (listenInput && IsGround) _VectorX = playerMoveInput;

        
        // 强制跳转
        if (_forceTranslateNextFixedUpdate)
        {
            ForceTranslate();
            _forceTranslateNextFixedUpdate = false;
            return;
        }


        // 重置环境矢量
        environmentMoveInput = 0f;
        
        // 矢量检查
        GravityCheck();
        // 头顶检测
        CheckCeilingCollision();
        // 移动平台检测
        CheckMobilePlatfrom();
        
        _VectorX += environmentMoveInput;
        
        // 最大矢量约束
        _VectorX = Math.Clamp(_VectorX, -HorizionalVelocityMax, HorizionalVelocityMax);
        _VectorY = Math.Clamp(_VectorY, -VerticalVelocityMax, VerticalVelocityMax);
        
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
    /// 只接收玩家的水平输入
    /// </summary>
    public void PlayerMove(float xInput)
    {
        playerMoveInput = xInput;
    }

    /// <summary>
    /// 跳跃力度输入
    /// </summary>
    /// <param name="jumpForce"></param>
    public void PlayerJump(float jumpForce)
    {
        _VectorY += jumpForce;
    }

    /// <summary>
    /// 强制切换位置，此执行将会延时到下一次FixedUpdated的时候执行
    /// </summary>
    /// <param name="position"></param>
    public void ForceTranslateTo(Vector3 position)
    {
        _forceTranslateNextFixedUpdate = true;
        _forceTranslatePosition = position;
    }

    /// <summary>
    /// fixdeUpdate里面运行
    /// </summary>
    private void ForceTranslate()
    {
        // 进行一次刷新
        _forceTranslatePosition = ObritMovementHelper.GetPositionOnCircle(_forceTranslatePosition, _ObritRadius);
        _characterController.transform.position = _forceTranslatePosition;
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
        if (_characterController.isGrounded && _VectorY < 0f)
        {
            if (IsGround == false)
            {
                _battleEventManager.TryGetNoArgEvent(BattleEventDefine.PLAYER_GROUNDED_START).Notify();
            }


            _VectorY = -0.1f;
            IsGround = true;

            return;
        }


        if (IsGround)
        {
            _battleEventManager.TryGetNoArgEvent(BattleEventDefine.PLAYER_GROUNDED_END).Notify();
        }

        // 叠加垂直矢量，重力是垂直向下的，所以需要减
        _VectorY -= Gravity * Time.deltaTime;
        IsGround = false;
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
    
    private void CheckMobilePlatfrom()
    {
        // Perform a raycast upwards from the character
        if (Physics.Raycast(transform.position, Vector3.down, out var hit, _characterController.height / 2 + ceilingCheckDistance, ceilingMask))
        {
            if (hit.collider.CompareTag("MovePlatform") && IsGround)
            {
                // 这种情况没办法处理自行转动的内容
                environmentMoveInput = hit.transform.GetComponent<PlatformOrbitMove>().speed;
            }
        }
    }

    // 比起被动的检测，对于跟随这种，不如主动发出射线检测
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _moveDirection = hit.moveDirection;

        if (_moveDirection.y == 0)
        {
            _VectorX = 0f;
        }
        //
        // if(hit.collider.CompareTag("MovePlatform") || IsGround)
        // {
        //     environmentMoveInput = hit.gameObject.GetComponent<PlatformObritMove>().speed;
        // }
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
            DrawMoveDirection();
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

    private void DrawMoveDirection()
    {
        Gizmos.DrawRay(transform.position,_moveDirection * 3);
    }

    #endregion
}
