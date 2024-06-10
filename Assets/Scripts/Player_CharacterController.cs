using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterControllerObritMove),typeof(PlayerEnquieController))]
public class Player_CharacterController : MonoBehaviour
{
    [SerializeField] 
    private ISListener _inputListener;
    
    /// <summary>
    /// 原本方案使用一个中心去做旋转，现在废弃。
    /// </summary>
    [SerializeField]
    // private Transform _PlayerLocator;
    
    [Header("Player Ability")]
    public float speed = 3f;

    // 当玩家所在地方不是战局的时候，不执行
    public bool canUseEnquie = true;

    [Header("Controller")] private CharacterControllerObritMove ccom;

    [Header("Enquie")] private PlayerEnquieController pec;

    public void Awake()
    {
        ccom = GetComponent<CharacterControllerObritMove>();
        pec = GetComponent<PlayerEnquieController>();
    }

    private void Start()
    {
        pec.Init(ccom);

        _inputListener.jumpEvent += JumpInputCallBack;
    }

    // TODO:重构。狗屎写法
    private void Update()
    {
        if (ccom != null)
        {
            ccom.moveInput = _inputListener.smoothMovement.x;

            if (ccom.IsGround)
            {
                pec.ResetEnquieSort();
            }

            if (ccom.jumpInput)
            {
                UseEnquie();
            }
        }
    }

    private void JumpInputCallBack()
    {
        if (ccom.IsGround)
        {
            Jump();
        }
        else
        {
            UseEnquie();
        }
    }

    // TODO:可以改成一个固定在第一位的装备。
    private void Jump()
    {
        ccom._VectorY += 10f;
        Debug.Log("Jump Player");
    }

    private void UseEnquie()
    {
        if(!canUseEnquie)return;

        pec.Use();
    }
}
