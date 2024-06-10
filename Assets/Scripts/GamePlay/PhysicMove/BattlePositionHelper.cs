using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattlePositionHelper
{
    /// <summary>
    /// 塔的起始位置
    /// </summary>
    public static Vector3 TowerCenterPosition;

    /// <summary>
    /// 塔的半径
    /// </summary>
    public static float TowerRadius;

    public static void SetupTower(Vector3 towerCenterPosition, float towerRadius)
    {
        TowerCenterPosition = towerCenterPosition;
        TowerRadius = towerRadius;
    }
}
