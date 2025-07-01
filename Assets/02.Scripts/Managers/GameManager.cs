using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoSingleton<GameManager>
{
    public DestinyData curDestinyData;// 현재 적용중인 운명

    public event Action onDestinyChange;

    private void Start()
    {
        TableManager.Instance.Init(this);
    }

    public void setCurDestinyData(DestinyData destinyData)
    {
        this.curDestinyData = destinyData;
        onDestinyChange?.Invoke();
    }
}
