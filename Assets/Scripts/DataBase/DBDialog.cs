/*
 * Author: CharSui
 * Created On: 2024.07.18
 * Description: 管理全局的对话获取
 * 这一步不需要获取多语言。只需要获取基础的配置信息即可。
 */

using System;
using System.Collections.Generic;
using System.IO;
using Basic.Defines;
using Dialog;
using Newtonsoft.Json;
using UnityEngine;

namespace DataBase
{
    public class DBDialog : ISerialize
    {
        private List<DialogProgress> _datas;

        public DBDialog()
        {
            _datas = new List<DialogProgress>(128);
        }

        /// <summary>
        /// 目前只打算做lalako一个人，所以直接一个List，适配多人需要改Dictionary
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public DialogProgress GetDialogProgress(string roleName, int progress)
        {
            if (roleName != RoleNameDefine.Lalako)
            {
                Debug.LogError($"[{nameof(DBDialog)}]如果你是有意而为，那么此时此刻该重构了");
                return default;
            }

            foreach (var dialogProgress in _datas)
            {
                var currentDialogProgress = dialogProgress;
                // 如果匹配进度，则返回
                if (currentDialogProgress.progress == progress)
                {
                    return currentDialogProgress;
                }
            }

            Debug.LogError($"[{nameof(DBDialog)}]无法找到适配进度的内容");
            return default;
        }

        /// <summary>
        ///     覆写初始化
        /// </summary>
        /// <param name="jsonData"></param>
        protected override void DeserializeOverride(string jsonData)
        {
            // 由于目前只打算做一个角色，所以只有一个Json。
            // 如果要适配多个角色，则需要改成字典，以及反序列化也要改。
            _datas.Clear();
            try
            {
                var data = JsonConvert.DeserializeObject<List<DialogProgress>>(jsonData);
                if (data == null)
                {
                    Debug.LogError($"[{nameof(DBDialog)}]Json数据没法解析");
                    return;
                }

                _datas = data;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            
        }
    }
}