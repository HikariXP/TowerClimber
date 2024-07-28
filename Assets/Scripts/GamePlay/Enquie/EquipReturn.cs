using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Skill;
using UnityEngine;

public class EquipReturn : IEquip
{
    private Queue<Vector3> _historyPosition;

    private int _historyMaxCount;

    private float _timePass = 0f;

    // 有一个很极致优化但不可视的优化：记录tick，比较int大小
    private float _recordCoolDown = 0.2f;

    // 回到5秒前
    private float _backTime = 3f;

    private RigidBodyOrbitMove rbom;

    public void Init(RigidBodyOrbitMove moveComponent)
    {
        rbom = moveComponent;

        var container = (int)Mathf.Floor(1 / _recordCoolDown * _backTime);
        _historyMaxCount = container;
        _historyPosition = new Queue<Vector3>(container);
    }

    public void Use(Vector3 position)
    {
        if (_historyPosition.Count <= 0)
        {
            return;
        }

        // 获取值，使用后清空历史
        var targetPosition = _historyPosition.Dequeue();
        _historyPosition.Clear();
        
        // 应用位置但不影响矢量
        rbom.ForceTranslateTo(targetPosition);
    }

    public void Update(float deltaTime)
    {
        _timePass += deltaTime;
        if (_timePass > _recordCoolDown)
        {
            var position = rbom.transform.position;

            // 如果超出需要自己排掉第一个元素，否则会自动增加
            if (_historyPosition.Count == _historyMaxCount)
            {
                // 丢弃掉第一个元素
                _historyPosition.Dequeue();
            }

            _historyPosition.Enqueue(position);
            _timePass = 0f;
            // Debug.Log($"[{nameof(Enquie_Return)}]Record:{position}");
        }
    }

    public void FixedUpdate(float deltaTime)
    {
        // Do Nothing
    }
}
