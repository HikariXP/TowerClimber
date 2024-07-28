/*
 * Author: CharSui
 * Created On: 2024.07.07
 * Description:全局多语言管理
 * 最小的表的单位是字典
 * 因为Addressable不能重名，所以不能通过文件夹导向的形式去读取。
 * 用Addressable读取需要同时保持程序集有ResourcesManager的引用，因为所用的异步句柄来源于其。
 * 通常而言，本地化包括以下需求：
 * 字体适配
 * 文案适配
 * 
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Localization
{
    public class LocalizationManager
    {
        private static LocalizationManager s_Instance;
        public static LocalizationManager instance => s_Instance;

        public static string currentLanguage;
        
        /// <summary>
        /// 根据{语言_模块,{key, localization}}
        /// </summary>
        private static Dictionary<string, Dictionary<string, string>> s_Data;

        private string[] _modules;
        
        // 通过事件回调，LocalizationManager只提供读取功能。
        // private static EventManager s_UIEventManager;

        public event Action OnLanguageChange;

        /// <summary>
        /// 初始化的时候传入模块列表，随后加载的时候会基于多语言作为前缀去寻找多语言列表
        /// </summary>
        /// <param name="moduleArray"></param>
        public void Init(string[] moduleArray)
        {
            s_Data = new Dictionary<string, Dictionary<string, string>>(16);
            _modules = moduleArray;

            currentLanguage = GetSystemLanguage();
            ResetLanguage(currentLanguage, _modules);

            s_Instance = this;
        }
        
        /// <summary>
        /// 获取多语言
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="localizationKey"></param>
        /// <returns></returns>
        public static string GetLocalization(string moduleName, string localizationKey)
        {
            // 无法获取模组
            if (string.IsNullOrEmpty(moduleName) || !s_Data.TryGetValue(moduleName, out var moduleLocalization))
            {
                Debug.LogError($"[{nameof(LocalizationManager)}]Can't Find Module with {moduleName}");
                return string.Empty;
            }

            // 无法获取Key
            if (string.IsNullOrEmpty(localizationKey) || !moduleLocalization.TryGetValue(localizationKey, out string localizationValue))
            {
                Debug.LogError($"[{nameof(LocalizationManager)}]Can't Find LocalizationKey with {localizationKey}");
                return string.Empty;
            }

            return localizationValue;
        }
        
        // 提供一个静态方法，将对应的string的对应内容转换成目标内容。(业务去做好了)
        // public static string Insert

        public void ResetLanguage(string language, string[] modules)
        {
            Debug.Log($"[{nameof(LocalizationManager)}]Reset Language to {language}");
            
            s_Data.Clear();
            
            foreach (var module in modules)
            {
                // 解析json
                string assetName = $"{module}_{language}";
                var jsonText = Addressables.LoadAssetAsync<TextAsset>(assetName).WaitForCompletion();
                var jsonList = JsonConvert.DeserializeObject<List<LocalizationStruct>>(jsonText.text);

                var container = new Dictionary<string, string>();
                // 写入模块对应的字典
                foreach (var localizationStruct in jsonList)
                {
                    container.Add(localizationStruct.key, localizationStruct.localizationText);
                }
                
                // 添加到最大索引里
                s_Data.Add(module, container);
            }
            
            OnLanguageChange?.Invoke();
        }

        private string GetSystemLanguage()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.Chinese:
                    return "ChineseSimple";
                
                // TODO:增加多语言
                default:
                    return "English";
            }
        }
    }

    public struct LocalizationStruct
    {
        [JsonProperty("key")]
        public string key;
        
        [JsonProperty("text")]
        public string localizationText;
        
        [JsonProperty("text_eng")]
        public string description;
    }
}