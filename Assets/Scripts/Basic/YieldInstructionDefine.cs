/*
 * Author: CharSui
 * Created On: 2024.07.14
 * Description: 一些常用的携程延迟提前定义
 */
using UnityEngine;

public static class YieldInstructionDefine
{
    public static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    public static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
}
