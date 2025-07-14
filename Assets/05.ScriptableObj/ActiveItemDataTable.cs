using UnityEngine;

[CreateAssetMenu(fileName = "ActiveItemDataTable", menuName = "Scriptable Objects/ActiveItemDataTable")]

public class ActiveItemDataTable : BaseTable<ActiveItemData>
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
