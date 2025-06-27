using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DestinyManager : MonoSingleton<DestinyManager>
{
    TableManager _tableManager;
    List<DestinyData> _destinyDatas;

    protected override bool Persistent => false;
    public void Init(TableManager tableManager)
    {
        _tableManager = tableManager;
        _destinyDatas = tableManager.GetTable<DestinyDataTable>().dataList;
    }
    public List<DestinyData> GetDestiny(int count)
    {
        return _destinyDatas.ShuffleData().Take(count).ToList();
    }
}
