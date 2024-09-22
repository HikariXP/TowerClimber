/*
 * Author: CharSui
 * Created On: 2024.08.22
 * Description: 
 */

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Tower
{
    public class TowerEvent : MonoBehaviour
    {
        [Header("Event Invoked Index"),ShowInInspector]
        private List<int> _eventIndexList;
    
        [LabelText("塔事件测试")]
        [Button("添加事件")]
        public void AddEventIndex(int index)
        {
        }

        public void RemoveEventIndex()
        {
        }
    }
}