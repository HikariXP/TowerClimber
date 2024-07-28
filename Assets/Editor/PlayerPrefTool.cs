/*
 * Author: CharSui
 * Created On: 2024.06.22
 * Description: 提供一些即时对PlayerPref的操作。
 */

using UnityEditor;
using UnityEngine;

public class PlayerPrefTool : EditorWindow
{
    // [MenuItem("EditorTool/PlayerPref/OpenToolWindow")]
    public void OpenWindow()
    {
        var window = GetWindow<PlayerPrefTool>();
        window.Show();
    }

    [MenuItem("EditorTool/PlayerPref/ClearAll")]
    public static void QuickClearPlayerPref()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log($"[{nameof(PlayerPrefTool)}]Invoke PlayerPrefs.DeleteAll()");
    }

    public void OnGUI()
    {
        
    }
}
