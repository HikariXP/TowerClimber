/*
 * Author: CharSui
 * Created On: 2024.05.26
 * Description: 提供一个Position让VirtualCamera跟随的GameObject
 * 在水平上保持和玩家高度一致
 */

using System;
using UnityEngine;

public class PlayerCameraOrbit : MonoBehaviour
{
    public float distance = 3f;

    /// <summary>
    /// 塔的水平坐标
    /// </summary>
    public Vector3 StartPosition = Vector3.zero;

    public Transform PlayerTransform;

    public float lerpSpeed = 1f;

    /// <summary>
    ///  此坐标不需要处理物理，放在update里面
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void Update()
    {

        // transform.position = targetPosition;
    }

    private void LateUpdate()
    {
        var position = PlayerTransform.position;
        
        StartPosition = new Vector3(StartPosition.x, position.y, StartPosition.z);
        var targetPosition = GetExtendedPoint(StartPosition, position,distance);
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed);
    }

    private Vector3 GetExtendedPoint(Vector3 startPoint, Vector3 endPoint, float distance)
    {
        // Step 1: Calculate the direction vector from start to end
        Vector3 direction = endPoint - startPoint;
        
        // Step 2: Normalize the direction vector to have a magnitude of 1 (unit vector)
        Vector3 normalizedDirection = direction.normalized;
        
        // Step 3: Scale the direction vector by the desired extension distance
        Vector3 extendedVector = normalizedDirection * distance;
        
        // Step 4: Add the scaled direction vector to the end point to get the extended point
        Vector3 extendedPoint = endPoint + extendedVector;
        
        return extendedPoint;
    }
}
