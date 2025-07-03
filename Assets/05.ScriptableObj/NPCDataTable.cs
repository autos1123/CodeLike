using UnityEngine;

[CreateAssetMenu(fileName = "NPCDataTable", menuName = "Scriptable Objects/NPCDataTable")]

public class NPCDataTable :BaseTable<NPCData>
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
