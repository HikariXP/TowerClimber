/*
 * Author: CharSui
 * Created On: 2024.07.21
 * Description: 对话系统的UI控制，包括输入监听以及内容显示
 */

using System.Collections.Generic;
using Basic.Interface;
using Dialog;
using Module.EventManager;
using TMPro;
using UnityEngine;

namespace UI.BattleUI
{
    public class PnlDialog : MonoBehaviour,IInitializable
    {
        public List<GameObject> rolePictureArchor;

        [Tooltip("对话的主体(比如角色名字)")] public TMP_Text txtDialogTitle;

        [Tooltip("对话的文本内容")] public TMP_Text txtDialogText;

        [Tooltip("控制消失")] public CanvasGroup canvasGroup;

        private EventManager _eventManager;

        // 测试阶段，之后需要做全局UI管理器
        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// 需要提前初始化
        /// </summary>
        public void Initialize()
        {
            _eventManager = EventHelper.GetEventManager(EventManagerType.UIEventManager);
            _eventManager.TryGetNoArgEvent(UIEventDefine.UI_DIALOG_START).Register(StartShowDialog);
        }

        private void StartShowDialog()
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            // 帮忙点击一次，以开启对话
            OnClick();
        }

        private void EndShowDialog()
        { 
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void OnClick()
        {
            // 尝试获取对话，默认会从0开始获取。
            if (!DialogManager.instance.TryGetNextDialog(out var dialog))
            {
                EndShowDialog();
                return;
            }
            
            var text = Localization.LocalizationManager.GetLocalization("role_lalako",dialog.dialogContentKey);
            txtDialogText.text = text;
        }
    }
}