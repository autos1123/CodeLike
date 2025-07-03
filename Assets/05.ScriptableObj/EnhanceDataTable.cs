using UnityEngine;

[CreateAssetMenu(fileName = "EnhanceDataTable", menuName = "Scriptable Objects/EnhanceDataTable")]

public class EnhanceDataTable : BaseTable<EnhanceData>
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
