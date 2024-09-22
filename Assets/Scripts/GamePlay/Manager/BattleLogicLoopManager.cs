/*
 * Author: CharSui
 * Created On: 2024.08.13
 * Description: 战局主循环
 */

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay
{
    public class BattleLogicLoopManager : MonoBehaviour
    {
        private static BattleLogicLoopManager _instance;
        public static BattleLogicLoopManager instance => _instance;
        
        /// <summary>
        /// 战局时间系数
        /// </summary>
        public static float battleRuntimeTimeScale = 0f;

        public static float battleRuntimeDeltaTime => Time.deltaTime * battleRuntimeTimeScale;
    
        /// <summary>
        /// 战局物理时间系数
        /// </summary>
        public static float battleRuntimeFixedTimeScale = 0f;

        public static float battleRuntimeFixedDeltaTime => Time.fixedDeltaTime * battleRuntimeFixedTimeScale;
        
        /// <summary>
        /// 战局在Update总控
        /// </summary>
        [ShowInInspector] private List<IPause> _updateObjects = new List<IPause>(16);
        
        /// <summary>
        /// 战局的Fixedupdate总控
        /// </summary>
        [ShowInInspector] private List<IPause> _fixUpdateObjects = new List<IPause>(16);
        
        public void Register(UpdateType updateType, IPause updateObject)
        {
            switch (updateType)
            {
                case UpdateType.Update:
                    _updateObjects.Add(updateObject);
                    break;
                
                case UpdateType.FixedUpdate:
                    _fixUpdateObjects.Add(updateObject);
                    break;
            }
        }

        public void Unregister(UpdateType updateType, IPause updateObject)
        {
            switch (updateType)
            {
                case UpdateType.Update:
                    _updateObjects.Remove(updateObject);
                    break;
                
                case UpdateType.FixedUpdate:
                    _fixUpdateObjects.Remove(updateObject);
                    break;
            }
        }
        
        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            for (int i = 0; i < _updateObjects.Count; i++)
            {
                _updateObjects[i].ControlledUpdate(battleRuntimeDeltaTime);
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < _fixUpdateObjects.Count; i++)
            {
                _fixUpdateObjects[i].ControlledUpdate(battleRuntimeFixedDeltaTime);
            }
        }

        private void Initialize()
        {
            _instance = this;
        }
    }

    public enum UpdateType
    {
        Update,
        FixedUpdate
    }
}