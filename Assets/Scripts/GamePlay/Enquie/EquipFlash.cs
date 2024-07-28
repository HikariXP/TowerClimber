/*
 * Author: CharSui
 * Created On: 2024.06.05
 * Description: 向目前朝向的水平方向突进一段距离。
 * 效果描述：重置垂直惯性，维持1秒的无视垂直力的突进
 * TODO:后期需要做成类似于获得一个Buff？
 */

using UnityEngine;

namespace Skill
{
    public class EquipFlash : IEquip
    {
        // 由于装备不是Monobehaviour，无法使用协程
        // private Coroutine m_FlashCoroutine;

        #region 道具属性

        private bool isActivity = false;

        private float _activityTime = 0.2f;

        private float _timeRemain = -0.02f;

        private float _minCheckTime = -0.01f;

        #endregion

        // 记得去思考怎么修正这个方案，理论上道具不应该控制ccom，而是应该告诉他让他去做而已。
        private RigidBodyOrbitMove rbom;
        
        public void Init(RigidBodyOrbitMove moveComponent)
        {
            rbom = moveComponent;
        }

        public void Use(Vector3 position)
        {
            if(_timeRemain > 0) return;

            Debug.Log($"[{nameof(EquipFlash)}]Enquie Flash use");
            _timeRemain = _activityTime;

            isActivity = true;
        }

        public void Update(float deltaTime)
        {
            if (_timeRemain < _minCheckTime) return;

            rbom.listenInput = false;
            // rbom._VectorY = 0;
            // rbom._VectorX = rbom._VectorX > 0f ? 30f : -30f;
            
            // TODO:这里会有大量新建的GC，需要关注一下
            var flashVectorX = rbom.GetVelocity().x > 0f ? 10f : -10f;
            rbom.SetVelocity(new Vector2(flashVectorX,0));
            
            _timeRemain -= deltaTime;

            if (_timeRemain < _minCheckTime)
            {
                isActivity = false;
                rbom.listenInput = true;
            }
        }

        public void FixedUpdate(float deltaTime)
        {
            // if(_timeRemain < _minCheckTime) return;
            //

        }
    }
}