using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleEventDefine
{
    public const uint PLAYER_GROUNDED_START = 0;
    
    public const uint PLAYER_GROUNDED_END = 1;

    /// <summary>
    /// 有可互动内容进入范围
    /// </summary>
    public const uint PLAYER_ITEM_IN_INTERACT_AREA = 2;
    
    /// <summary>
    /// 有可互动内容退出范围
    /// </summary>
    public const uint PLAYER_ITEM_OUT_INTERACT_AREA = 3;

    // =======================================================================================================================================================================================
    // 1000开始是互动，前面预留一千个单位给战局活动
    
    /// <summary>
    /// 玩家跟角色互动
    /// </summary>
    public const uint PLAYER_INTERACT_ROLE = 1000;


    public const uint PLAYER_TELEPORT = 1010;


}
