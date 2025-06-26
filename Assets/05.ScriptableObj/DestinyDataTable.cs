using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DestinyDataTable", menuName = "Scriptable Objects/DestinyDataTable")]

public class DestinyDataTable : BaseTable<DestinyData>
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
