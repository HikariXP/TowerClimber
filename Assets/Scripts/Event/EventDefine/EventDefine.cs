using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleEventDefine
{
    public const uint PLAYER_GROUNDED_START = 0;
    
    public const uint PLAYER_GROUNDED_END = 1;

    // 1000开始是互动，前面预留一千个单位给战局活动
    public const uint PLAYER_INTERACT_ROLE = 1000;
}
