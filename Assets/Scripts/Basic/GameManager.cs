/*
 * Author: CharSui
 * Created On: 2024.06.23
 * Description: 全局游戏管理器，负责一些基础的暂停，加载，初始化之类的活。
 */

using System.Collections;
using DataBase;
using Dialog;
using Localization;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 执行顺序放到最前，有必要的话去ProjectSetting里面调整ScriptOrder
    /// </summary>
    private void Awake()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        Debug.Log($"[{nameof(GameManager)}]Start Init");
        // 玩家存档初始化
        PlayerSave playerSave = new PlayerSave();
        playerSave.Load();
        
        // yield return YieldInstructionDefine.waitForEndOfFrame;
        
        // 玩家战利品缓存
        PlayerItem playerItem = new PlayerItem();
        // yield return YieldInstructionDefine.waitForEndOfFrame;

        // 多语言初始化
        string[] modules =
        {
            // 应该根据Label直接获取
            // 目前的多语言方案不太适配Addressable的读取方案。后续如果出移动端可以考虑接入YooAsset
            // UI
            "base_ui",
            
            // 对话
            "role_lalako"
        };
        new LocalizationManager().Init(modules);
        
        yield return YieldInstructionDefine.waitForEndOfFrame;
        
        new DatabaseManager().Init();

        new DialogManager();
        
        Debug.Log($"[{nameof(GameManager)}]Init Finish");
    }
    
    
}
