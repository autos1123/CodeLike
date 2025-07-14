using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataTable", menuName = "Scriptable Objects/ItemDataTable")]

public class ItemDataTable :BaseTable<ItemData>

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
