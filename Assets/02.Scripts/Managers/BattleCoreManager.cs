using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-99)]
public class BattleCoreManager : MonoSingleton<BattleCoreManager>
{
    GameManager gameManager;
    PlayerManager playerManager;
    PoolManager poolManager;
    ItemManager itemManager;//아이템매니저는 위치 고민필요
    StageManager stageManager;

    public PlayerManager PlayerManager {  get { return playerManager; } }
    public PoolManager PoolManager { get { return poolManager; } }
    public ItemManager ItemManager { get { return itemManager; } }
    public StageManager StageManager { get { return stageManager; } }

    public DestinyData curDestinyData;// 현재 적용중인 운명

    public event Action onDestinyChange;
    protected  override bool Persistent => false;
    protected override void Awake()
    {
        base.Awake();
        gameManager = GameManager.Instance;

        playerManager = PlayerManager.Instance;

        poolManager = PoolManager.Instance;
        itemManager = ItemManager.Instance;
        //stageManager

        //TOdo : 매니저들 추가 init();
        //itemManager.Init(gameManager.TableManager);
    }

    public void setCurDestinyData(DestinyData destinyData)
    {
        this.curDestinyData = destinyData;
        onDestinyChange?.Invoke();
    }

}
