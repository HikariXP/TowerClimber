/*
 * Author: CharSui
 * Created On: 2024.08.05
 * Description: 管理当前游戏的大部分基础内容
 * 比如：玩家复活、场景跳转
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Config.ScriptableObject;
using GamePlay;
using GamePlay.Tower;
using Module.EventManager;
using Sirenix.OdinInspector;
using UnityEngine;

public class BattleEnvironmentManager : MonoBehaviour
{
    private static BattleEnvironmentManager s_instance;
    public static BattleEnvironmentManager instance => s_instance;
    
    private bool isInited;

    // TODO:塔实例控制器
    private Tower currentTower;

    // TODO：战局控制单位管理器
    private GameObject currentPlayer;

    // TODO:摄像机控制管理器
    private GameObject vc;
    private GameObject vcHelper;
    
    private int nextTowerIndex;
    private Vector3 nextSpawnPointPosition;

    /// <summary>
    /// 是否正在传送
    /// </summary>
    private bool isTeleporting = false;

    /// <summary>
    /// 通过堆栈的方式，按照顺序加载物体以及卸载物体
    /// </summary>
    private Stack<GameObject> objectLoaded;

    [Header("Develop-TowerDefine")]
    public List<GameObject> devTowerList = new List<GameObject>(8);

    public List<Tower> devTowers = new List<Tower>(8);

    public GameObject playerPrefab;

    public ISListener IsListener;

    public GameObject VirtualCamera;
    public GameObject VirtualCameraArchor;

    /// <summary>
    /// 测试开发阶段，直接指定塔来进行初始化
    /// </summary>
    [Header("Debug")]
    public Tower debugTower;

    private void Start()
    {
        Initialize();
        Teleport(0);
    }

    public void Initialize()
    {
        if(isInited)return;
        
        // TODO：改发布模式
        devTowers.Clear();
        foreach (var tower in devTowerList)
        {
            var towerComponent = tower.GetComponent<Tower>();
            if(towerComponent == null) continue;
            
            devTowers.Add(towerComponent);
        }

        RegisterEvent();
    }

    private void RegisterEvent()
    {
        EventHelper.GetEventManager(EventManagerType.BattleEventManager)
            .TryGetArgEvent<TowerTeleportInfo>(BattleEventDefine.PLAYER_TELEPORT).Register(Teleport);
    }

    private void UnregisterEvent()
    {
        EventHelper.GetEventManager(EventManagerType.BattleEventManager)
            .TryGetArgEvent<TowerTeleportInfo>(BattleEventDefine.PLAYER_TELEPORT).Unregister(Teleport);
    }

    /// <summary>
    /// 前往下一区域
    /// </summary>
    private void ToArea(Vector3 spawnPointPosition, int towerIndex)
    {
        nextTowerIndex = towerIndex;
        nextSpawnPointPosition = spawnPointPosition;
    }

    private void Teleport(TowerTeleportInfo tti)
    {
        var spawnPointIndex = tti.nextSpawnPointIndex;
        Teleport(spawnPointIndex);
    }
    
    private void Teleport(int spawnPointIndex)
    {
        if (isTeleporting)
        {
            Debug.Log($"[{nameof(BattleEnvironmentManager)}]Repreat Teleport, try later");
            return;
        }

        StartCoroutine(E_Teleport(spawnPointIndex));
    }

    [Button]
    public void ForceInitialize()
    {
        Initialize();
    }
    
    [Button]
    public void DEV_Teleport(int spawnPointIndex)
    {
        Teleport(spawnPointIndex);
    }

    /// <summary>
    /// 是不是应该做成，生成的是一批，生成完了，在一个地方提供引用，然后这些组件在逐级进行初始化？
    /// </summary>
    /// <param name="spawnPointIndex"></param>
    /// <returns></returns>
    private IEnumerator E_Teleport(int spawnPointIndex)
    {
        // 卸载 ================================================================================================
        
        // 注销摄像机
        if (vc != null)
        {
            Destroy(vc);
            
        }        
        if (vcHelper != null)
        {
            Destroy(vcHelper);
        }
        
        yield return new WaitForSeconds(0.2f);

        if (currentPlayer != null)
        {
            // 注销输入
            var currentPlayerComponent = currentPlayer.GetComponent<Player>();
            currentPlayerComponent.UnregisterInputListener();
            
            // 注销循环订阅且暂定
            var currentPlayerRigidBodyComponent = currentPlayer.GetComponent<RigidBodyOrbitMove>();
            BattleLogicLoopManager.instance.Unregister(UpdateType.FixedUpdate, currentPlayerRigidBodyComponent);
            
            Destroy(currentPlayer);
        }
        yield return new WaitForSeconds(0.2f);
        
        BattleLogicLoopManager.battleRuntimeFixedTimeScale = 0f;
        BattleLogicLoopManager.battleRuntimeTimeScale = 0f;

        yield return new WaitForSeconds(1f);
        
        if (currentTower != null) Destroy(currentTower.gameObject);
        
        // 加载 ==================================================================================================
        
        Debug.Log($"[{nameof(BattleEnvironmentManager)}]Start Teleport:{spawnPointIndex}");
        yield return new WaitForSeconds(0.2f);
        
        // 1、初始化塔
        TowerSpawnPoint tsp = null;
        Tower t = null;
        
        
        // TODO:后续由准确的配置表读取
        foreach (var tower in devTowers)
        {
            if (!tower.TryGetTowerSpawnPoint(spawnPointIndex, out var towerSpawnPoint))
            {
                continue;
            }
            
            currentTower = Instantiate(tower.gameObject).GetComponent<Tower>();
            t = tower;
            tsp = towerSpawnPoint;
        }

        if (tsp == null || t == null)
        {
            yield break;
        }
        
        var isInRoom = t.isInRoom;
        
        // 2、初始化玩家
        yield return new WaitForSeconds(0.2f);
        
        var player = Instantiate(playerPrefab);
        var playerComponent = player.GetComponent<Player>();
        var playerRigidBodyComponent = player.GetComponent<RigidBodyOrbitMove>();
        playerComponent.Initialize();
        playerComponent.RegisterInputListener(IsListener);
        
        player.transform.position = tsp.position;
        
        playerRigidBodyComponent.SetupObritInfo(t.radius);
        playerRigidBodyComponent.isInRoom = isInRoom;

        currentPlayer = player;
        
        BattleLogicLoopManager.instance.Register(UpdateType.FixedUpdate, playerRigidBodyComponent);
        BattleLogicLoopManager.battleRuntimeFixedTimeScale = 1f;
        BattleLogicLoopManager.battleRuntimeTimeScale = 1f;

        // 3、初始化摄像机
        yield return new WaitForSeconds(0.2f);
        var VC_MainFollowCamera = Instantiate(VirtualCamera);
        var VC = VC_MainFollowCamera.GetComponent<CinemachineVirtualCamera>();

        vc = VC_MainFollowCamera;

        var archor = Instantiate(VirtualCameraArchor);
        var archorComponent = archor.GetComponent<PlayerLookAtCircleCameraOrbit>();
        
        vcHelper = archor;

        VC.Follow = archor.transform;
        VC.LookAt = player.transform;
        archorComponent.followObject = player.transform;
        archorComponent.isInRoom = t.isInRoom;
        
        // 4、切换InputMap
    }
}
