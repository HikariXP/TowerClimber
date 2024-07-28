/*
 * Author: CharSui
 * Created On: 2024.07.17
 * Description: 全局数据库初始化和刷新管理
 */

using System;
using System.Collections.Generic;
using Basic.Defines;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DataBase
{
    public class DatabaseManager
    {
        private static DatabaseManager s_Instance;
        public static DatabaseManager instance => s_Instance;
        
        private readonly Dictionary<Type, object> _databases = new (16);

        public DatabaseManager()
        {
            if (s_Instance != null)
            {
                Debug.LogError($"[{nameof(DatabaseManager)}]初始化重复，检查逻辑，单例已存在");
            }

            s_Instance = this;
        }

        public void Init()
        {
            Debug.Log($"[{nameof(DatabaseManager)}]初始化开始");
            _databases.Clear();

            // 初始化对话数据库
            {
                var jsonText = Addressables.LoadAssetAsync<TextAsset>($"{RoleNameDefine.Lalako}_dialog").WaitForCompletion();
                DBDialog dbDialog = new DBDialog();
                dbDialog.Deserialize(jsonText.text);
                _databases.Add(typeof(DBDialog),dbDialog);
            }
        }

        public T GetDataBase<T>() where T : class
        {
            if (_databases.TryGetValue(typeof(T), out object database))
            {
                return database as T;
            }

            return null;
        }
    }

}
