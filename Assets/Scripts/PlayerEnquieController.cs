/*
 * Author: CharSui
 * Created On: 2024.05.30
 * Description: 玩家的装备管理器
 */

using System;
using System.Collections.Generic;
using Skill;
using UnityEngine;

// 不一定需要MonoBehaviour，也可以用来显示
public class PlayerEnquieController : MonoBehaviour
{
    // 由于使用是有顺序的，可以考虑做成单链表
    public List<IEnquie> Enquies;

    private int _enquieIndex = 0;

    private RigidBodyObritMove rbom;

    private float _enquieUseCoolDown = 0.3f;

    private float _timePass = 0f;

    private float _minCheckTime = -0.01f;
    
    
    private void Update()
    {
        var updateTime = Time.deltaTime;
        if (_timePass > _minCheckTime)
        {
            _timePass -= updateTime;
        }

        for (int i = 0; i < Enquies.Count; i++)
        {
            Enquies[i].Update(updateTime);
        }
    }

    private void FixedUpdate()
    {
        var fixedTime = Time.deltaTime;
        for (int i = 0; i < Enquies.Count; i++)
        {
            Enquies[i].FixedUpdate(fixedTime);
        }
    }

    public void Init(RigidBodyObritMove rbom)
    {
        this.rbom = rbom;
        
        // 测试阶段先写死。
        Enquies = new List<IEnquie>();
        
        // 玩家没有跳跃这个能力，是通过道具去跳跃，道具第一位固定是跳跃去赋予玩家"具有跳跃能力"的结果
        Enquies.Add(new Enquie_DoubleJump());
        
        Enquies.Add(new Enquie_DoubleJump());
        Enquies.Add(new Enquie_Flash());
        Enquies.Add(new Enquie_Return());

        for (int i = 0; i < Enquies.Count; i++)
        {
            Enquies[i].Init(rbom);
        }

        _enquieIndex = 0;
    }

    public void Use()
    {
        if(_timePass > _minCheckTime)return;

        _timePass = _enquieUseCoolDown;
        
        Debug.Log($"[{nameof(PlayerEnquieController)}]{_enquieIndex}");
        
        // 空的话就拿第一个
        var currentSelectEnquie = Enquies[_enquieIndex];
        
        currentSelectEnquie.Use(transform.position);
        
        _enquieIndex += 1;
        if (_enquieIndex >= Enquies.Count) _enquieIndex = 0;
    }
    
    public void ResetEnquieSort()
    {
        _enquieIndex = 0;
    }

}
