/*
 * Author: CharSui
 * Created On: 2024.05.30
 * Description: 二段跳的装备
 */

using System.Collections;
using UnityEngine;

namespace Skill
{
    public class EquipDoubleJump : IEquip
    {
        private RigidBodyOrbitMove rbom;

        /// <summary>
        /// 理论上装备不应该包含这个。
        /// </summary>
        private float jumpForce = 10f;

        private float timeRemain = 0f;

        private float effectTime = 1;

        public void Init(RigidBodyOrbitMove moveComponent)
        {
            rbom = moveComponent;
            // 改为获取玩家的统一属性。
            jumpForce = moveComponent.JumpForce;
            // 数据来源需要重新配置
        }

        public void Use(Vector3 position)
        {
            rbom.checkGounded = false;
            
            timeRemain = 0f;
            
            // Debug.Log($"[{nameof(Enquie_DoubleJump)}]Double Jump use");
            var originVelocity = rbom.GetVelocity();
            rbom.SetVelocity(new Vector2(originVelocity.x, jumpForce));
        }

        public void Update(float deltaTime)
        {
            if (timeRemain < effectTime && timeRemain > -0.1f)
            {
                timeRemain += deltaTime;
            }
            else
            {
                timeRemain = -0.2f;
                rbom.checkGounded = true;
            }
        }

        public void FixedUpdate(float deltaTime)
        {
            // Do nothing
        }
    }
}