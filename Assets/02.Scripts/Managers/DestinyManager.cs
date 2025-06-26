using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DestinyManager : MonoSingleton<DestinyManager>
{
    TableManager tableManager;
    List<DestinyData> destinyDatadatas;

    protected override bool Persistent => false;
    public void Init()
    {
        tableManager = TableManager.Instance;
        destinyDatadatas = tableManager.GetTable<DestinyDataTable>().dataList;

    }
    public List<DestinyData> GetDestiny(int count)
    {
        return destinyDatadatas.ShuffleData().Take(count).ToList();
    }
}
