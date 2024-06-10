using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Skill;
using UnityEngine;

public class Enquie_Return : IEnquie
{
    /// <summary>
    /// TODO : 似乎可以优化为根据冷却去动态判断这个容量
    /// </summary>
    private Queue<Vector3> _historyPosition;

    private float _timePass = 0f;

    private float _recordCoolDown = 0.5f;

    // 回到5秒前
    private float _backTime = 5f;

    private CharacterControllerObritMove ccom;

    public void Init(CharacterControllerObritMove moveComponent)
    {
        ccom = moveComponent;

        var container = (int)Mathf.Floor(1 / _recordCoolDown * _backTime);
        _historyPosition = new Queue<Vector3>(container);
    }

    public void Use(Vector3 position, float vectorX, float vectorY)
    {
        if (_historyPosition.Count <= 0)
        {
            return;
        }

        var targetPosition = _historyPosition.Dequeue();
        _historyPosition.Clear();
        ccom.transform.position = targetPosition;
    }

    public void Update(float deltaTime)
    {
        _timePass += deltaTime;
        if (_timePass > _recordCoolDown)
        {
            var position = ccom.transform.position;
            _historyPosition.Enqueue(position);
            _timePass = 0f;
            Debug.Log($"[{nameof(Enquie_Return)}]Record:{position}");
        }
    }

    public void FixedUpdate(float deltaTime)
    {
        // Do Nothing
    }
}
