/*
 * Author: CharSui
 * Created On: 2024.06.22
 * Description: 游戏里的道具基本都是些和剧情有关的碎片道具，全局唯一的。
 */

using Sirenix.OdinInspector;
using UnityEngine;

public class InteractiveLoot : IInteractiveUnit
{
    /// <summary>
    /// 战利品序号
    /// </summary>
    [Header("LootSetting")]
    public uint LootIndex;

    public override void Init(uint newPriority)
    {
        canRepeatInteract = false;

        base.Init(newPriority);
    }

    public override void Interact()
    {
        if(!CanInteract())return;
        
        haveInteract = true;
        Debug.Log($"[{nameof(InteractiveLoot)}]Interact with LootIndex:{LootIndex}");
        PlayerItem.instance.AddItem(LootIndex);
        
        gameObject.SetActive(false);
    }
    
#if UNITY_EDITOR

    /// <summary>
    /// 展示对应LootIndex的信息
    /// </summary>
    [Button]
    public void EditorShowItemInfo()
    {
        
    }

#endif
}
