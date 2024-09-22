/*
 * Author: CharSui
 * Created On: 2024.08.06
 * Description: 塔的基础信息
 * 塔的半径，对应位置的生成点和描述
 */

using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Tower
{
    public class TowerSpawnPoint : MonoBehaviour
    {
        /// <summary>
        /// 全局Index
        /// </summary>
        public int spawnPointIndex;

        /// <summary>
        /// 所在塔的Index
        /// </summary>
        public int towerIndex;

        /// <summary>
        /// 复活坐标点
        /// </summary>
        public Vector3 position;

        /// <summary> 
        /// 复活点描述
        /// </summary>
        public string description;
        
        #if UNITY_EDITOR

        [Button("收集当前世界坐标信息")]
        public void SetPosition()
        {
            position = transform.position;
        }

#endif
    }
}