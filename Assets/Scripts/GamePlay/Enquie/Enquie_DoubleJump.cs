/*
 * Author: CharSui
 * Created On: 2024.05.30
 * Description: 二段跳的装备
 */

using UnityEngine;

namespace Skill
{
    public class Enquie_DoubleJump : IEnquie
    {
        private CharacterControllerObritMove ccom;

        /// <summary>
        ///     理论上装备不应该包含这个。
        /// </summary>
        private float jumpForce = 10f;

        public void Init(CharacterControllerObritMove moveComponent)
        {
            ccom = moveComponent;
            jumpForce = moveComponent.JumpForce;
            // 数据来源需要重新配置
        }

        public void Use(Vector3 position, float vectorX, float vectorY)
        {
            // Debug.Log($"[{nameof(Enquie_DoubleJump)}]Double Jump use");
            ccom._VectorY = jumpForce;
        }

        public void Update(float deltaTime)
        {
            // Do nothing
        }

        public void FixedUpdate(float deltaTime)
        {
            // Do nothing
        }
    }
}