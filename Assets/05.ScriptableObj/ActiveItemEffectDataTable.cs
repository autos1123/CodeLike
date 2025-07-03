using UnityEngine;

[CreateAssetMenu(fileName = "ActiveItemEffectDataTable", menuName = "Scriptable Objects/ActiveItemEffectDataTable")]

public class ActiveItemEffectDataTable :BaseTable<ActiveItemEffectData>
{
    public override void CreateTable()
    {
        base.CreateTable();
        foreach(var data in dataList)
        {
            DataDic[data.SkillID] = data;
        }
    }

}
