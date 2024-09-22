/*
 * Author: CharSui
 * Created On: 2024.05.20
 * Description: 这种计算会涉及极坐标的计算。
 * TODO：之后改成指定的按键触发对应的技能。
 */
using System;
using System.Collections;
using System.Collections.Generic;
using Module.EventManager;
using UnityEngine;

[RequireComponent(typeof(Rigidbody),typeof(RigidBodyOrbitMove))]
public class Player : MonoBehaviour
{
    [SerializeField] 
    private ISListener _inputListener;

    [Header("Player Ability")]
    public float speed = 3f;

    public float JumpForce = 10f;

    // 当玩家所在地方不是战局的时候，不执行
    public bool canUseEnquie = true;

    [Header("Controller")] private RigidBodyOrbitMove rbom;

    [Header("Enquie")] private PlayerEnquieController pec;

    [Header("Interact")] private PlayerInteractor pi;

    private void Start()
    {

    }

    private void OnDestroy()
    {
        _inputListener.jumpEvent -= JumpInputCallBack;
        
        var battleEventManager = EventHelper.GetEventManager(EventManagerType.BattleEventManager);
        battleEventManager.TryGetNoArgEvent(BattleEventDefine.PLAYER_GROUNDED_START).Unregister(ResetEnquie);
    }

    // TODO:重构。狗屎写法
    private void Update()
    {
        if (rbom == null)
        {
            Debug.LogError($"[{nameof(Player)}]You are using a controller with nothing to control");
            return;
        }
        
        // 传入输入
        rbom.PlayerMove(_inputListener.smoothMovement.x * speed);
    }

    public void Initialize()
    {
        rbom = GetComponent<RigidBodyOrbitMove>();
        pec = GetComponent<PlayerEnquieController>();
        pi = GetComponent<PlayerInteractor>();
        
        pec.Init(rbom);
        
        
        var battleEventManager = EventHelper.GetEventManager(EventManagerType.BattleEventManager);
        battleEventManager.TryGetNoArgEvent(BattleEventDefine.PLAYER_GROUNDED_START).Register(ResetEnquie);
    }

    /// <summary>
    /// 注册控制器
    /// </summary>
    public void RegisterInputListener(ISListener listener)
    {
        if (listener == null)
        {
            Debug.LogError($"[{nameof(Player)}]You are trying to register a null inputListener");
            return;
        }

        // 如果已存在控制器，则先注销
        if (_inputListener != null)
        {
            UnregisterInputListener();
        }

        _inputListener = listener;
        _inputListener.jumpEvent += JumpInputCallBack;
    }

    /// <summary>
    /// 注销当前正在使用的控制器
    /// </summary>
    public void UnregisterInputListener()
    {
        if(_inputListener == null) return;
        
        _inputListener.jumpEvent -= JumpInputCallBack;
    }

    private void JumpInputCallBack()
    {
        //最好还是区分跳跃和道具。更符合主观以及出bug好查。
        // 初次跳跃还有触地检查的短暂停止。触发跳跃后，会先关停地面检测
        
        if(Interact())return;
        
        // if(Jump())return;

        UseEnquie();
    }

    /// <summary>
    /// 触发互动
    /// </summary>
    /// <returns></returns>
    private bool Interact()
    {
        return pi.TryInteract();
    }

    /// <summary>
    /// 触发道具的使用
    /// </summary>
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
