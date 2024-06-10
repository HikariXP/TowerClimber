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

    private CharacterControllerObritMove ccom;

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

    public void Init(CharacterControllerObritMove ccom)
    {
        this.ccom = ccom;
        
        // 测试阶段先写死。
        Enquies = new List<IEnquie>();
        Enquies.Add(new Enquie_DoubleJump());
        Enquies.Add(new Enquie_Flash());
        Enquies.Add(new Enquie_Return());

        for (int i = 0; i < Enquies.Count; i++)
        {
            Enquies[i].Init(ccom);
        }

        _enquieIndex = 0;
    }

    public void Use()
    {
        if(_timePass > _minCheckTime)return;

        _timePass = _enquieUseCoolDown;
        
        // 空的话就拿第一个
        var currentSelectEnquie = Enquies[_enquieIndex];
        
        currentSelectEnquie.Use(transform.position,ccom._VectorX,ccom._VectorY);
        
        _enquieIndex += 1;
        if (_enquieIndex >= Enquies.Count) _enquieIndex = 0;
        
        Debug.Log($"[{nameof(PlayerEnquieController)}]{_enquieIndex}");
    }
    
    public void ResetEnquieSort()
    {
        _enquieIndex = 0;
    }

}
