/*
 * Author: CharSui
 * Created On: 2024.05.26
 * Description: 塔的基础信息
 */

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Tower
{
    public class Tower : MonoBehaviour
    {
        [Header("Tower Basic Config")]
        public int towerIndex;
        
        [Tooltip("塔的半径")]
        public float radius;

        [Tooltip("摄像机半径")]
        public float cameraRadius;

        /// <summary>
        /// 是否室内
        /// </summary>
        public bool isInRoom;

        [SerializeField] 
        private TowerEvent _towerEvent;

        [Header("Tower Spawn Point")] 
        public List<TowerSpawnPoint> towerSpawnPoints;

        [Header("Interact Object")] 
        public List<IInteractiveUnit> towerInteractObjects;


        public bool TryGetTowerSpawnPoint(int towerSpawnPointIndex, out TowerSpawnPoint towerSpawnPoint)
        {
            foreach (var currentTowerSpawnPoint in towerSpawnPoints)
            {
                if (currentTowerSpawnPoint.spawnPointIndex == towerSpawnPointIndex)
                {
                    towerSpawnPoint = currentTowerSpawnPoint;
                    return true;
                }
            }

            towerSpawnPoint = default;
            return false;
        }
        
        #if UNITY_EDITOR
        
        [Button("获取所有本塔的可互动设备")]
        public void GetTowerInteractObjects()
        {
            var units = GetComponentsInChildren<IInteractiveUnit>();
            if (units is not { Length: >= 1 }) return;
            
            towerInteractObjects.Clear();
            towerInteractObjects.AddRange(units);
        }

        [Button("获取所有本塔的复活点")]
        public void GetTowerSpawnPoint()
        {
            var units = GetComponentsInChildren<TowerSpawnPoint>();
            if (units is not { Length: >= 1 }) return;
            
            towerSpawnPoints.Clear();
            towerSpawnPoints.AddRange(units);

            foreach (var towerSpawnPoint in towerSpawnPoints)
            {
                towerSpawnPoint.towerIndex = towerIndex;
            }
        }



#endif
    }
}
