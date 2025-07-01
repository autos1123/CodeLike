using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DestinyEffectDataTable", menuName = "Scriptable Objects/DestinyEffectDataTable")]

public class DestinyEffectDataTable :BaseTable<DestinyEffectData>
{
    public override void CreateTable()
    {
        base.CreateTable();
        foreach(var data in dataList)
        {
            DataDic[data.ID] = data;
        }
    }

}
