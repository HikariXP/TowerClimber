/*
 * Author: CharSui
 * Created On: 2024.06.30
 * Description: 调试器，对调试的内容进行了一层简单的包装
 */

using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Object = System.Object;

[Obsolete("使用这种方式去做Log，会导致双击log跳转的地方并不是出bug的地方")]
public class CharSuiDebuger
{
    private static StringBuilder s_StringBuilder = new StringBuilder();

    [Obsolete]
    public static void Log(Object obj, string logContent)
    {
        Debug.Log(MergeContent(obj, logContent));
    }
    
    [Obsolete]
    public static void LogError(Object obj, string logContent)
    {
        Debug.LogError(MergeContent(obj, logContent));
    }
    
    private static string MergeContent(Object obj, string logContent)
    {
        s_StringBuilder.Clear();
        s_StringBuilder.Append($"[{obj.GetType().Name}] ");
        s_StringBuilder.Append(logContent);
        return s_StringBuilder.ToString();
    }
}
