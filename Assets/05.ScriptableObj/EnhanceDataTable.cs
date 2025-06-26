using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnhanceDataTable", menuName = "Scriptable Objects/EnhanceDataTable")]

public class EnhanceDataTable : BaseTable<EnemyData>
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
