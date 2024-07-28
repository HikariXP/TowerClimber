/*
 * Author: CharSui
 * Created On: 2024.05.30
 * Description: 道具接口,使用的时候基本都是操作玩家的[矢量]以及[位置]
 */

using UnityEngine;

namespace Skill
{
    public interface IEquip
    {
        /// <summary>
        /// 用于初始化一些信息
        /// </summary>
        /// <param name="player"></param>
        public void Init(RigidBodyOrbitMove moveComponent);

        /// <summary>
        /// 使用接口,位置、矢量
        /// </summary>
        public void Use(Vector3 position);

        /// <summary>
        /// 处理一些计时
        /// </summary>
        public void Update(float deltaTime);

        /// <summary>
        /// 处理一些记录相关的内容
        /// </summary>
        public void FixedUpdate(float deltaTime);
    }
}