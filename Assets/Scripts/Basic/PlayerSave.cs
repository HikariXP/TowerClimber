/*
 * Author: CharSui
 * Created On: 2024.06.23
 * Description: 玩家存档管理
 */

using System;
using UnityEngine;

[Obsolete("正式需要使用之前，直接使用PlayerPref")]
public class PlayerSave
{
     // TODO:做成可以适配手机的。也就是支持多种保存方式的。
     // 或者从头开始就采用json的格式去保存
     public static PlayerSave instance{ get; private set; }

     public PlayerSave()
     {
         Load();

         instance = this;
     }

     public void Save()
     {
         PlayerPrefs.Save();
     }
     
     public void Load()
     {
         
         // 没有这个函数，PlayerPrefs在进入游戏的时候自动初始化
         // PlayerPrefs.Load();
     }
}
