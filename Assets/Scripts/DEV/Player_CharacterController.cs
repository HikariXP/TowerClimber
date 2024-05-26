using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_CharacterController : MonoBehaviour
{
    [SerializeField] 
    private ISListener _inputListener;
    
    /// <summary>
    /// 原本方案使用一个中心去做旋转，现在废弃。
    /// </summary>
    [SerializeField]
    // private Transform _PlayerLocator;

    private double _radius = 11d;

    /// <summary>
    /// 是否着地，会同时影响互动
    /// </summary>
    [Header("重力配置")]
    public bool isGround;
    
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    private Transform GroundChecker;

    private float airTime;

    private float gravity = 9.8f;
    
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    
    [Header("Player Ability")]
    public double speed = 3d;
    public float speed_Float = 3f;

    public float jumpForce = 10f;

    [Header("Test")] public RigidBodyObritMove rbom;
    [Header("Test")] public CharacterControllerObritMove ccom;

    private void Start()
    {
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    private void Update()
    {
        // 更新信息给移动控制
        if (rbom != null)
        {
            rbom.moveInput = _inputListener.smoothMovement.x * speed_Float;
            rbom.jumpInput = _inputListener.jump;   
        }

        if (ccom != null)
        {
            ccom.moveInput = _inputListener.smoothMovement.x * speed_Float;
            ccom.jumpInput = _inputListener.jump ? jumpForce : 0;
        }
    }

    private void GroundCheck()
    {
        // set sphere position, with offset
        var position = GroundChecker.position;
        Vector3 spherePosition = new Vector3(position.x, position.y - GroundedOffset,
            position.z);
        isGround = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
    }
    
    private void UseEnquie()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        isGround = true;
    }
    
    private void OnCollisionExit(Collision collision)
    {
        isGround = false;
    }
}
