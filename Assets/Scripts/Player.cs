/*
 * Author: CharSui
 * Created On: 2024.05.20
 * Description: 这种计算会涉及极坐标的计算。
 */
using System;
using System.Collections;
using System.Collections.Generic;
using Module.EventManager;
using UnityEngine;

[RequireComponent(typeof(Rigidbody),typeof(RigidBodyObritMove))]
public class Player : MonoBehaviour
{
    [SerializeField] 
    private ISListener _inputListener;

    private EventManager _battleEventManager;

    [Header("Player Ability")]
    public float speed = 3f;

    public float JumpForce = 10f;

    // 当玩家所在地方不是战局的时候，不执行
    public bool canUseEnquie = true;

    [Header("Controller")] private RigidBodyObritMove rbom;

    [Header("Enquie")] private PlayerEnquieController pec;

    public void Awake()
    {
        rbom = GetComponent<RigidBodyObritMove>();
        pec = GetComponent<PlayerEnquieController>();
        
        _battleEventManager = EventHelper.GetEventManager(EventManagerType.BattleEventManager);
    }
    
    private void Start()
    {
        pec.Init(rbom);

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
        if (rbom == null)
        {
            Debug.LogError($"[{nameof(Player_CharacterController)}]You are using a controller with nothing to control");
            return;
        }
        
        // 传入输入
        rbom.PlayerMove(_inputListener.smoothMovement.x * speed);
        
        
    }

    private void JumpInputCallBack()
    {
        //最好还是区分跳跃和道具。更符合主观以及出bug好查。
        // 初次跳跃还有触地检查的短暂停止。触发跳跃后，会先关停地面检测
        UseEnquie();
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
