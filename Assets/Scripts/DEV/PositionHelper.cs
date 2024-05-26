/*
 * Author: CharSui
 * Created On: 2024.05.23
 * Description: 获取一些位置信息以及方向的帮助
 */

using UnityEngine;

public static class PositionHelper 
{
    public static void CalculateDirection(Vector3 centerPosition, Vector3 objectPosition)
    {
        // 获得不算Y轴的点
        Vector3 flatPosition = new Vector3(centerPosition.x, 0, centerPosition.z);
        Vector3 flatTransformPosition = new Vector3(objectPosition.x, 0, objectPosition.z);

        // 获取两点的距离
        float distance = Vector3.Distance(flatPosition, flatTransformPosition);

        // Calculate the direction vector from transform to position on the XZ plane
        Vector3 direction = (flatPosition - flatTransformPosition).normalized;

        // Calculate the tangent direction for circular motion
        Vector3 circularDirection = Vector3.Cross(direction, Vector3.up).normalized;

        // Debug lines to visualize in the Scene view (optional)
        Debug.DrawLine(objectPosition, objectPosition + direction * distance, Color.red, 1.0f);
        Debug.DrawLine(objectPosition, objectPosition + circularDirection * distance, Color.green, 1.0f);

        // Print out the results
        Debug.Log("Distance on XZ plane: " + distance);
        Debug.Log("Direction towards position (XZ plane): " + direction);
        Debug.Log("Circular motion direction (XZ plane): " + circularDirection);
    }
}
