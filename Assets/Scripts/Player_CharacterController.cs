using System;
using System.Collections;
using System.Collections.Generic;
using Module.EventManager;
using UnityEngine;

[RequireComponent(typeof(CharacterControllerObritMove),typeof(PlayerEnquieController))]
public class Player_CharacterController : MonoBehaviour
{
    [SerializeField] 
    private ISListener _inputListener;

    private EventManager _battleEventManager;
    
    /// <summary>
    /// 原本方案使用一个中心去做旋转，现在废弃。
    /// </summary>
    [SerializeField]
    // private Transform _PlayerLocator;
    
    [Header("Player Ability")]
    public float speed = 3f;

    public float JumpForce = 10f;

    // 当玩家所在地方不是战局的时候，不执行
    public bool canUseEnquie = true;

    [Header("Controller")] private CharacterControllerObritMove ccom;

    [Header("Enquie")] private PlayerEnquieController pec;

    public void Awake()
    {
        ccom = GetComponent<CharacterControllerObritMove>();
        pec = GetComponent<PlayerEnquieController>();
        
        _battleEventManager = EventHelper.GetEventManager(EventManagerType.BattleEventManager);
    }
    
    private void Start()
    {
        // pec.Init(ccom);

        _inputListener.jumpEvent += JumpInputCallBack;

        _battleEventManager.TryGetNoArgEvent(BattleEventDefine.PLAYER_GROUNDED_START).Register(ResetEnquie);
    }

    private void OnDestroy()
    {
        _inputListener.jumpEvent -= JumpInputCallBack;
        
        _battleEventManager.TryGetNoArgEvent(BattleEventDefine.PLAYER_GROUNDED_START).Unregister(ResetEnquie);
    }

    // TODO:重构。狗屎写法
    private void Update()
    {
        if (ccom == null)
        {
            Debug.LogError($"[{nameof(Player_CharacterController)}]You are using a controller with nothing to control");
            return;
        }
        
        // 传入输入
        ccom.PlayerMove(_inputListener.smoothMovement.x * speed);

        // 弃用，用事件处理
        // if (ccom.IsGround)
        // {
        //     pec.ResetEnquieSort();
        // }
    }

    private void JumpInputCallBack()
    {
        UseEnquie();
    }

    // TODO:可以改成一个固定在第一位的装备。
    // 且
    private void Jump()
    {
        // ccom.PlayerJump(JumpForce);
    }

    private void UseEnquie()
    {
        if(!canUseEnquie)return;

        pec.Use();
    }

    private void ResetEnquie()
    {
        pec.ResetEnquieSort();
    }
}
