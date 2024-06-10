/*
 * Author: CharSui
 * Created On: 2024.05.20
 * Description: 这种计算会涉及极坐标的计算。
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] 
    private ISListener _inputListener;

    /// <summary>
    /// 是否着地，会同时影响互动
    /// </summary>
    [Header("重力配置")]
    public bool isGround;

    private Transform GroundChecker;

    private float airTime;

    private float gravity = 9.8f;

    [Header("Player Ability")]
    public float speed = 3f;

    // public float jumpForce = 10f;
    [Header("Controller")]
    public RigidBodyObritMove rbom;

    private void Update()
    {
        // 更新信息给移动控制
        rbom.moveInput = _inputListener.smoothMovement.x * speed;
        // rbom.jumpInput = _inputListener.jump;
        
        isGround = rbom.isGround;
        UseEnquieCheck();
    }

    private void UseEnquieCheck()
    {
        // 地面没法使用道具
        if(isGround) return;
        
        // 还没起跳，没法使用道具
        // if (!_inputListener.jump)return;
        
        Debug.Log("Use a Item");
    }
}
