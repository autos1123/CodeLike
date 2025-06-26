using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCoreManager : MonoSingleton<BattleCoreManager>
{
    PlayerManager playerManager;
    DestinyManager destinyManager;
    PoolManager poolManager;
    ItemManager itemManager;//아이템매니저는 위치 고민필요
    StageManager stageManager;

    public PlayerManager PlayerManager {  get { return playerManager; } }
    public DestinyManager DestinyManager { get { return destinyManager; } }
    public PoolManager PoolManager { get { return poolManager; } }
    public ItemManager ItemManager { get { return itemManager; } }
    public StageManager StageManager { get { return stageManager; } }

    protected override void Awake()
    {
        base.Awake();

        playerManager = PlayerManager.Instance;
        destinyManager = DestinyManager.Instance;
        poolManager = PoolManager.Instance;
        itemManager = ItemManager.Instance;
        //stageManager

        //TOdo : 매니저들 추가 init();

    }

}
