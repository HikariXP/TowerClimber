/*
 * Author: CharSui
 * Created On: 2024.08.20
 * Description: 运行时测试获取PlayerPref数据
 */

using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerPrefRuntimeDebuger : MonoBehaviour
{
    [Button]
    public void GetSting(string key)
    {
        var result = PlayerPrefs.GetString(key);
        Debug.Log($"[{nameof(PlayerPrefRuntimeDebuger)}]String:{result}");
    }

    [Button]
    public void GetInt(string key)
    {
        var result = PlayerPrefs.GetInt(key);
        Debug.Log($"[{nameof(PlayerPrefRuntimeDebuger)}]Int:{result}");
    }
    
    [Button]
    public void GetFloat(string key)
    {
        var result = PlayerPrefs.GetFloat(key);
        Debug.Log($"[{nameof(PlayerPrefRuntimeDebuger)}]Float:{result}");
    }

    [Button]
    public void HasKey(string key)
    {
        var result = PlayerPrefs.HasKey(key);
        Debug.Log($"[{nameof(PlayerPrefRuntimeDebuger)}]HasKey:{result}");
    }
}
