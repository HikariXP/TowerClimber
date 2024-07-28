/*
 * Author: CharSui
 * Created On: 2024.06.23
 * Description: 道具管理器。本身游戏功能需求简单，就随便做了。
 * 注意这里需要区别：这里管理的只是藏品，一些跟故事相关的碎片道具。这些道具基本都只会有一份。
 */

using System.Collections.Generic;
using Sirenix.OdinInspector;


public class PlayerItem
{
    /// <summary>
    /// 由主管理器初始化
    /// </summary>
    public static PlayerItem instance { get; private set; }

    [ReadOnly,ShowInInspector]
    private List<uint> _items;

    public PlayerItem()
    {
        instance = this;
        Init();
    }

    public void Init()
    {
        _items = new List<uint>();
        // 加载存档
    }

    public void Release()
    {
        _items = null;
        instance = null;
    }

    public void AddItem(uint itemIndex)
    {
        
    }

    public void RemoveItem(uint itemIndex)
    {
        
    }

    public void GetAllItem()
    {
        
    }

    public void ClearItem()
    {
        _items.Clear();
    }


#if UNITY_EDITOR
    [Button]
    public void EditorAddItem(uint itemIndex)
    {
        
    }

    public void EditorRemoveItem(uint itemIndex)
    {
        
    }

#endif
}
