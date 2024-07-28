/*
 * Author: CharSui
 * Created On: 2024.05.30
 * Description: 玩家的装备管理器
 */

using System.Collections.Generic;
using Skill;
using UnityEngine;

// 不一定需要MonoBehaviour，也可以用来显示
public class PlayerEnquieController : MonoBehaviour
{
    // 由于使用是有顺序的，可以考虑做成单链表
    public List<IEquip> _equips;

    private int _enquieIndex = 0;

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

        // 这里每帧都跑，我认为用foreach不好
        for (int i = 0; i < _equips.Count; i++)
        {
            _equips[i].Update(updateTime);
        }
    }

    private void FixedUpdate()
    {
        var fixedTime = Time.deltaTime;
        for (int i = 0; i < _equips.Count; i++)
        {
            _equips[i].FixedUpdate(fixedTime);
        }
    }

    public void Init(RigidBodyOrbitMove rbom)
    {
        // 测试阶段先写死。
        _equips = new List<IEquip>();
        
        // 玩家没有跳跃这个能力，是通过道具去跳跃，道具第一位固定是跳跃去赋予玩家"具有跳跃能力"的结果
        _equips.Add(new EquipDoubleJump());
        
        _equips.Add(new EquipDoubleJump());
        _equips.Add(new EquipFlash());
        _equips.Add(new EquipReturn());

        for (int i = 0; i < _equips.Count; i++)
        {
            _equips[i].Init(rbom);
        }

        _enquieIndex = 0;
    }

    public void Use()
    {
        if(_timePass > _minCheckTime)return;

        _timePass = _enquieUseCoolDown;
        
        
        
        // 空的话就拿第一个
        var currentSelectEnquie = _equips[_enquieIndex];
        
        currentSelectEnquie.Use(transform.position);
        Debug.Log($"[{nameof(PlayerEnquieController)}]使用道具{_enquieIndex}");
        
        _enquieIndex += 1;
        if (_enquieIndex >= _equips.Count) _enquieIndex = 0;
    }
    
    public void ResetEnquieSort()
    {
        _enquieIndex = 0;
    }

}
