/*
 * Author: CharSui
 * Created On: 2024.06.22
 * Description: 玩家互动器，但是不清楚是玩家主动检测好，还是由道具去主动检测好。
 * 毕竟每种内容的触发效果都不一样
 */

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    private const string tag_interactive = "Interactive";

    private RigidBodyOrbitMove playerMoveComponent;

    [ShowInInspector]
    private readonly List<IInteractiveUnit> _unitCanInteract = new(8);

    private void Awake()
    {
        if (playerMoveComponent == null)
        {
            playerMoveComponent = GetComponent<RigidBodyOrbitMove>();
        }
    }

    //看情况如果需要转换模式：由可互动内容去告知玩家可互动，可以将此改为Public供其调用
    private void AddUnit(IInteractiveUnit unit)
    {
        _unitCanInteract.Add(unit);
    }

    private void RemoveUnit(IInteractiveUnit unit)
    {
        _unitCanInteract.Remove(unit);
    }

    public bool TryInteract()
    {
        if (_unitCanInteract.Count == 0) return false;

        var unit = _unitCanInteract[0];

        unit.Interact(playerMoveComponent);
        _unitCanInteract.Remove(unit);
        return true;
    }

    #region Physics-物理检测
    
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag(tag_interactive))return;

        var interactiveUnit = other.GetComponentInParent<IInteractiveUnit>();

        if(interactiveUnit == null) return;
        
        // 如果自动触发的，在玩家进入就自动触发，且不算入可互动列表
        if (interactiveUnit.autoInteract)
        {
            interactiveUnit.Interact(playerMoveComponent);
            return;
        }

        AddUnit(interactiveUnit);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag(tag_interactive))return;
        
        RemoveUnit(other.GetComponentInParent<IInteractiveUnit>());
    }
    
    #endregion
}