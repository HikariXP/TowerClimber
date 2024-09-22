/*
 * Author: CharSui
 * Created On: 2024.06.23
 * Description: 触发NPC对话
 */

using Module.EventManager;
using UnityEngine;

public class InteractiveNPC : IInteractiveUnit
{
    public string roleName;
    
    public override void Interact(RigidBodyOrbitMove _)
    {
        // 触发对话系统与哪个角色对话
        Debug.Log($"[{nameof(InteractiveNPC)}]Interact with {roleName}");
        
        EventHelper.GetEventManager(EventManagerType.BattleEventManager).TryGetArgEvent<string>(BattleEventDefine.PLAYER_INTERACT_ROLE).Notify(roleName);
    }
}
