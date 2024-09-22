/*
 * Author: CharSui
 * Created On: 2024.06.22
 * Description: 用于处理跨场景的移动
 * TODO:玩家=》互动器不够扩展性，比如一些门锁上的旧比较难搞。
 */

using Module.EventManager;
using UnityEngine;

public class InteractiveTeleport : IInteractiveUnit
{
    [Header("Teleport")]
    public string nextPointDescription;

    /// <summary>
    /// 下一座塔的id
    /// </summary>
    public int nextTowerIndex;
    
    /// <summary>
    /// 下一座个传送口位置
    /// </summary>
    public int nextSpawnPointIndex;
    
    public override void Interact(RigidBodyOrbitMove _)
    {
        Debug.Log($"[InteractiveTeleport]TeleportTo TowerIndex:{nextTowerIndex}, SpawnPointIndex:{nextSpawnPointIndex}");
        var translateInfo = new TowerTeleportInfo
        {
            nextTowerIndex = this.nextTowerIndex,
            nextSpawnPointIndex = this.nextSpawnPointIndex
        };

        EventHelper.GetEventManager(EventManagerType.BattleEventManager).TryGetArgEvent<TowerTeleportInfo>(BattleEventDefine.PLAYER_TELEPORT).Notify(translateInfo);
    }
}