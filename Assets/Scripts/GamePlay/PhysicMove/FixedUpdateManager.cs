/*
 * Author: CharSui
 * Created On: 2024.06.11
 * Description: 全局唯一物理逻辑处理管理
 * TODO:此举将重写整个物理处理逻辑。工作量会增加。好处是FixedUpdate的顺序将是可控的。
 */

using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Physic
{
    public class FixedUpdateManager : MonoBehaviour
    {
        // 如果后续需求增加，应该修改这个初始容量
        private readonly List<PhysicUpdatable> _physicUpdatables = new(128);

        private int _containerCount;

        private void FixedUpdate()
        {
            var deltaTime = Time.deltaTime;
            
            for (var i = 0; i < _containerCount; i++)
            {
                _physicUpdatables[i].PhysicUpdate(deltaTime);
            }
        }

        public void AddPhysicUpdate(PhysicUpdatable physicUpdatable)
        {
            _physicUpdatables.Add(physicUpdatable);
            _physicUpdatables.Sort();
            
            _containerCount = _physicUpdatables.Count;
        }

        public void RemovePhysicUpdate(PhysicUpdatable physicUpdatable)
        {
            _physicUpdatables.Remove(physicUpdatable);
            _physicUpdatables.Sort();
            
            _containerCount = _physicUpdatables.Count;
        }

        /// <summary>
        /// 理论上而言，应该是用不上的
        /// </summary>
        public void ClearPhysicUpdate()
        {
            _physicUpdatables.Clear();
            _containerCount = _physicUpdatables.Count;
        }
    }
}