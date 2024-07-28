/*
 * Author: CharSui
 * Created On: 2024.06.22
 * Description: 可交互的内容
 */

using System;
using UnityEngine;

public abstract class IInteractiveUnit : MonoBehaviour, IComparable<IInteractiveUnit>
{
    /// <summary>
    /// 同时进入可互动范围的时候的互动优先度
    /// </summary>
    [Header("InteractSetting")]
    public uint priority;

    /// <summary>
    /// 是否可以重复互动
    /// </summary>
    public bool canRepeatInteract;

    /// <summary>
    /// 是否碰到就自动触发
    /// </summary>
    public bool autoInteract;

    /// <summary>
    /// 是否已经互动
    /// </summary>
    public bool haveInteract;

    public virtual void Init(uint newPriority)
    {
        priority = newPriority;
    }

    /// <summary>
    /// 抽象互动接口
    /// </summary>
    public abstract void Interact();

    protected bool CanInteract()
    {
        return !haveInteract || canRepeatInteract;
    }

    // 用来处理优先度更高的互动，但是这种做法没有提供给玩家选择，需要对可互动的内容做设计。
    public int CompareTo(IInteractiveUnit other)
    {
        if (this.priority < other.priority) return -1;

        if (this.priority > other.priority) return 1;

        return 0;
    }
}
