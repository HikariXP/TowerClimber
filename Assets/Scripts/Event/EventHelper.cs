/*
 * Author: CharSui
 * Created On: 2024.06.09
 * Description: 提供全局的事件访问
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.EventManager
{
    public enum EventManagerType
    {
        BattleEventManager = 0,
        UIEventManager = 1,
    }

    public static class EventHelper
    {
        private static bool _isInited;
        
        // 初始化容器
        private static readonly EventManager[] s_EventManagers = new EventManager[8];

        public static void Refresh()
        {
            FillContainer();

            _isInited = true;
        }

        public static EventManager GetEventManager(EventManagerType type)
        {
            if (!_isInited) Refresh();
            
            var index = (int)type;
            return s_EventManagers[index];
        }
        
        private static void FillContainer()
        {
            for (int i = 0 ; i < s_EventManagers.Length ; i++)
            {
                s_EventManagers[i] = new EventManager();
            }
        }
    }
}

