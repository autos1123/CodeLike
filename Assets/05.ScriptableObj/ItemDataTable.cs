using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataTable", menuName = "Scriptable Objects/ItemDataTable")]

public class ItemDataTable :BaseTable<ItemData>

{
    public override void CreateTable()
    {
        base.CreateTable();
        
        Debug.Log($"[CreateTable] dataList.Count = {dataList.Count}");

        foreach(var data in dataList)
        {
            Debug.Log($"[CreateTable] 등록 중: ID={data.ID}");
            DataDic[data.ID] = data;
        }
    }
}
