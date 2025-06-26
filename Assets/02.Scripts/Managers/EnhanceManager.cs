using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnhanceManager :MonoSingleton<EnhanceManager>
{
    TableManager _tableManager;
    List<DestinyData> destinyDatadatas;
    public void Init(TableManager tableManager)
    {
        _tableManager = tableManager;
        destinyDatadatas = _tableManager.GetTable<DestinyDataTable>().dataList;
    }
    public List<DestinyData> GetDestiny(int count)
    {
        System.Random rand = new System.Random();// 이거는 위치가 이상한거 같음
        return destinyDatadatas.ShuffleData(rand).Take(count).ToList();
    }

}
