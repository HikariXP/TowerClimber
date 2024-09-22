/*
 * Author: CharSui
 * Created On: 2024.08.06
 * Description: 塔的信息读取
 * 塔的复活点是唯一的。有总塔信息，
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using GamePlay.Tower;

namespace DataBase
{
    // public class DBTower : ISerialize
    // {
    //     private List<GamePlay.Tower> _datas;
    //
    //     private List<TowerSpawnPosition> _subDatas;
    //
    //     public DBTower()
    //     {
    //         _datas = new List<TowerStruct>(128);
    //     }
    //
    //     /// <summary>
    //     /// 覆写初始化
    //     /// </summary>
    //     /// <param name="jsonData"></param>
    //     protected override void DeserializeOverride(string jsonData)
    //     {
    //         // 由于目前只打算做一个角色，所以只有一个Json。
    //         // 如果要适配多个角色，则需要改成字典，以及反序列化也要改。
    //         _datas.Clear();
    //         try
    //         {
    //             var data = JsonConvert.DeserializeObject<List<TowerStruct>>(jsonData);
    //             if (data == null)
    //             {
    //                 Debug.LogError($"[{nameof(DBDialog)}]Json数据没法解析");
    //                 return;
    //             }
    //
    //             _datas = data;
    //         }
    //         catch (Exception e)
    //         {
    //             Debug.LogError(e);
    //         }
    //         
    //     }
    // }
}