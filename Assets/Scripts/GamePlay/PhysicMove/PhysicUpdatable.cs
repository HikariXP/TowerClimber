/*
 * Author: CharSui
 * Created On: 2024.06.11
 * Description: 基础物理单位，相关的基本都需要Rigidbody
 */

using System;
using UnityEngine;

namespace GamePlay.Physic
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class PhysicUpdatable : MonoBehaviour, IComparable<PhysicUpdatable>
    {
        private uint _priority;

        private PhysicUpdatable(uint priority)
        {
            _priority = priority;
        }

        internal abstract void PhysicUpdate(float deltaTime);

        /// <summary>
        /// IDE自己生成的比较
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PhysicUpdatable other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return _priority.CompareTo(other._priority);
        }
    }
}