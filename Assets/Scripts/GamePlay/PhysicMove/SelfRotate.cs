/*
 * Author: CharSui
 * Created On: 2024.06.10
 * Description: 只用于外观上的自旋转
 */
using UnityEngine;

public class SelfRotate : MonoBehaviour
{
    // 每秒转动角度
    public float angleSpeed = 30f;
    
    private void Update()
    {
        // 计算每帧的旋转角度
        float rotationAngle = angleSpeed * Time.deltaTime;

        // 使用 Rotate 方法进行旋转
        transform.Rotate(0, rotationAngle, 0);
    }
}
