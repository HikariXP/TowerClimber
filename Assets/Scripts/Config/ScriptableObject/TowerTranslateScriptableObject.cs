/*
 * Author: CharSui
 * Created On: 2024.06.30
 * Description: 
 */

using UnityEngine;

namespace Config.ScriptableObject
{
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
        public int nextTower;
    }
}