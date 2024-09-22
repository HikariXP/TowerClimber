/*
 * Author: CharSui
 * Created On: 2024.08.13
 * Description: 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleLogicLoopHelper
{
    /// <summary>
    /// 战局时间系数
    /// </summary>
    public static float battleRuntimeTimeScale = 1f;

    public static float battleRuntimeDeltaTime => Time.deltaTime * battleRuntimeTimeScale;
    
    /// <summary>
    /// 战局物理时间系数
    /// </summary>
    public static float battleRuntimeFixedTimeScale = 1f;

    public static float battleRuntimeFixedDeltaTime => Time.fixedDeltaTime * battleRuntimeFixedTimeScale;
}
