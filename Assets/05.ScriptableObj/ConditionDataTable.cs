using UnityEngine;

[CreateAssetMenu(fileName = "ConditionDataTable", menuName = "Scriptable Objects/ConditionDataTable")]

public class ConditionDataTable :BaseTable<ConditionData>
{
    public override void CreateTable()
    {
        base.CreateTable();
        DataDic.Clear();
        foreach(var data in dataList)
        {
            DataDic[data.ID] = data;
        }
    }

}
