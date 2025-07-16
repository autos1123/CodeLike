using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestinyManager : MonoSingleton<DestinyManager>
{
    public DestinyData curDestinyData;// 현재 적용중인 운명

    public event Action<DestinyData, int> onDestinyChange;

    protected override bool Persistent => false;

    public void setCurDestinyData(DestinyData destinyData)
    {
        if(curDestinyData.ID != 0) onDestinyChange?.Invoke(curDestinyData, -1);

        this.curDestinyData = destinyData;
        onDestinyChange?.Invoke(curDestinyData, 1);
    }

}
