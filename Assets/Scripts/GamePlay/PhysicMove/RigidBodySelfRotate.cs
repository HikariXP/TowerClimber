/*
 * Author: CharSui
 * Created On: 2024.06.10
 * Description: 物理自转，和PlatformObritMove用的时一样的逻辑
 *
 * 注意事项：曾经是通过角速度去算，但是发现在传给Player的EnvironmentVelocity的时候会有误差。
 */

using UnityEngine;

namespace GamePlay.PhysicMove
{
    public class RigidBodySelfRotate : MonoBehaviour
    {
        private Rigidbody _rigidbody;

        public float playerObritRadius = 11f;

        public float speed = 10f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            // 计算每帧的旋转角度
            float rotationAngle = (speed / playerObritRadius) * Mathf.Rad2Deg * Time.fixedDeltaTime;

            // 计算新的旋转
            Quaternion deltaRotation = Quaternion.Euler(0, rotationAngle, 0);

            // 使用 MoveRotation 方法进行旋转
            _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
        }
    
        public void OnCollisionStay(Collision collisionInfo)
        {
            foreach (ContactPoint contact in collisionInfo.contacts)
            {
                // 获取碰撞的法线
                Vector3 normal = contact.normal;

                // 检查法线是否垂直于地面（假设Y轴为垂直方向）
                // 如果法线与垂直方向（Vector3.up）之间的点积接近1或-1，则说明几乎是垂直的。
                // 可以设置一个阈值来判断是否大致垂直，例如0.9或更高。
                if (Mathf.Abs(Vector3.Dot(normal, Vector3.up)) > 0.9f)
                {
                    var rbom = contact.otherCollider.GetComponent<RigidBodyObritMove>();
                    if(!rbom)return;
                
                    rbom.SetEnvironmentVelocity(-Vector2.right * speed);
                }
                else
                {
                    Debug.Log("碰撞法线非垂直");
                }
            }

            // 旧方案对垂直无效，且对于水平碰撞的时候，玩家会被弹飞。
            // var rbom = collisionInfo.body.GetComponent<RigidBodyObritMove>();
            // var temp = rbom.GetVelocity();
            // temp.x += speed;
            // rbom.SetVelocity(temp);
        }
    }
}
