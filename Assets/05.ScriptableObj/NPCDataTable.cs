using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCDataTable", menuName = "Scriptable Objects/NPCDataTable")]

public class NPCDataTable :BaseTable<NPCData>
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
