/*
 * Author: CharSui
 * Created On: 2024.08.05
 * Description: 控制一些输入显示，比如接近可互动物品的时候，会显示"互动"
 */

using System;
using Module.EventManager;
using UnityEngine;

public class PnlInteract : MonoBehaviour
{
    public void Start()
    {
        var eventManager = EventHelper.GetEventManager(EventManagerType.BattleEventManager);
        eventManager.TryGetArgEvent<InteractDefine.InteractType>(BattleEventDefine.PLAYER_ITEM_IN_INTERACT_AREA);
    }
}
