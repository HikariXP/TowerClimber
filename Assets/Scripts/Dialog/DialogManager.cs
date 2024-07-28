/*
 * Author: CharSui
 * Created On: 2024.07.15
 * Description: 表达对话可以用dialog和dialogue，但是不用英文了啦，全部统称dialog
 * 管理器：需要有初始化获取对话，也需要对应的加载。
 * 发起对话后，提供类似单链表的对话内容获取
 * 通过事件吊起对应的UIControl去显示当前的对话内容。
 *
 * 为了开发速度：此处将【对话进度记录】，以及【对话管理】融合到此类
 */
using System.Collections;
using System.Collections.Generic;
using Basic.Defines;
using DataBase;
using Module.EventManager;
using UnityEngine;

namespace Dialog
{
    public class DialogManager
    {
        private static DialogManager s_Instance;
        public static DialogManager instance => s_Instance;
        
        /// <summary>
        /// 当前正在进行的对话
        /// </summary>
        private DialogProgress _dialogProgressReading;
        
        /// <summary>
        /// 考虑做成协程
        /// </summary>
        private int _dialogIndex = 0;
        private int _maxDialogIndex = 0;

        public DialogManager()
        {
            if (s_Instance != null)
            {
                Debug.LogError($"[{nameof(DialogManager)}]重复初始化");
                return;
            }

            s_Instance = this;

            UnregisterEvent();
            RegisterEvent();
        }

        public void Refresh()
        {
            
        }

        private void RegisterEvent()
        {
            var battleEventManager = EventHelper.GetEventManager(EventManagerType.BattleEventManager);
            var uiEventManager = EventHelper.GetEventManager(EventManagerType.UIEventManager);
            
            // 触发对话:string = role_index
            battleEventManager.TryGetArgEvent<string>(BattleEventDefine.PLAYER_INTERACT_ROLE).Register(StartDialog);
            // uiEventManager.TryGetNoArgEvent(UIEventDefine.UI_DIALOG_CLICK).Register(TryGetNextDialog);

        }

        private void UnregisterEvent()
        {
            var battleEventManager = EventHelper.GetEventManager(EventManagerType.BattleEventManager);
            var uiEventManager = EventHelper.GetEventManager(EventManagerType.UIEventManager);
            
            battleEventManager.TryGetArgEvent<string>(BattleEventDefine.PLAYER_INTERACT_ROLE).Unregister(StartDialog);
            // uiEventManager.TryGetNoArgEvent(UIEventDefine.UI_DIALOG_CLICK).Register(TryGetNextDialog);
        }

        public void StartDialog(string roleName)
        {
            // 测试，实际投入使用应该有存档管理进度
            StartDialog(roleName, 0);
        }

        /// <summary>
        /// 用作特别情况，播放指定的对话
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="progress"></param>
        public void StartDialog(string roleName, int progress)
        {
            // 根据角色和进度获取对话。
            Debug.Log($"[{nameof(DialogManager)}]Start Dialog with {roleName}_{progress}");

            var dialogProgress = DatabaseManager.instance.GetDataBase<DBDialog>()
                .GetDialogProgress(RoleNameDefine.Lalako,progress);

            _dialogProgressReading = dialogProgress;
            
            _dialogIndex = 0;
            _maxDialogIndex = dialogProgress.dialogContext.Length;
            
            // 加载完成，唤起UI管理系统
            EventHelper.GetEventManager(EventManagerType.UIEventManager).TryGetNoArgEvent(UIEventDefine.UI_DIALOG_START).Notify();
            // 开始协程开始等待对话结束。
            // 或者直接DialogManager管理进度读取。
        }

        public bool TryGetNextDialog(out Dialog nextDialog)
        {
            if (_dialogIndex == _maxDialogIndex)
            {
                nextDialog = default;
                return false;
            }

            Dialog currentDialog = _dialogProgressReading.dialogContext[_dialogIndex];
            nextDialog = currentDialog;
            
            _dialogIndex += 1;
            return true;
        }
    }
}


