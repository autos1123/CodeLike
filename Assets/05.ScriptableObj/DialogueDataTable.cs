using UnityEngine;

[CreateAssetMenu(fileName = "DialogueDataTable", menuName = "Scriptable Objects/DialogueDataTable")]

public class DialogueDataTable :BaseTable<DialogueData>
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
