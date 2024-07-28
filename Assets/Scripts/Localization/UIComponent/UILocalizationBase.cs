/*
 * Author: CharSui
 * Created On: 2024.07.14
 * Description: 多语言抽象类，主要定义一些事件以及注册
 */

using System;
using UnityEngine;

namespace Localization
{
    public abstract class UILocalizationBase : MonoBehaviour
    {
        /// <summary>
        /// 由LocalizationManager去获取多语言
        /// </summary>
        public string localizationKey;

        private void Start()
        {
            Init();
        }

        public void OnEnable()
        {
            RefreshLocalization();
        }
        
        protected virtual void Init()
        {
            LocalizationManager.instance.OnLanguageChange += RefreshLocalization;
        }

        /// <summary>
        /// 由事件触发，刷新的事件又或者是Enable
        /// </summary>
        private void RefreshLocalization()
        {
            // TODO: 由于存在程序集，无法预设业务定义的模组名，需要优化
            string localizationText = LocalizationManager.GetLocalization("base_ui", localizationKey);
            RefreshLocalization(localizationText);
        }

        /// <summary>
        /// 基于业务直接触发的刷新
        /// </summary>
        /// <param name="localizationText"></param>
        protected abstract void RefreshLocalization(string localizationText);
    }
}
