/*
 * Author: CharSui
 * Created On: 2024.06.30
 * Description: 
 */

using System;
using UnityEngine;

namespace Config.ScriptableObject
{
    [Obsolete("新的替换为TowerTeleportInfo")]
    [CreateAssetMenu(menuName= "TC_Config/TowerTranslate",fileName = "NewTowerTranslate",order = 0)]
    public class TowerTranslateScriptableObject : UnityEngine.ScriptableObject
    {
        public TowerTranslateScriptableObject()
        {
            
        }
    }

    public struct TowerTranslateInfo
    {
        /// <summary>
        /// 下一座塔的ID，如果是-1则不传送
        /// </summary>
        public int nextTowerIndex;

        public int nextSpawnPointIndex;
    }
}