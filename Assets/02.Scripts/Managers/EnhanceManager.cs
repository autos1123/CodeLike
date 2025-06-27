using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnhanceManager :MonoSingleton<EnhanceManager>
{
    TableManager _tableManager;
    List<EnhanceData> _enhanceDatas;
    public void Init(TableManager tableManager)
    {
        _tableManager = tableManager;
        _enhanceDatas = _tableManager.GetTable<EnhanceDataTable>().dataList;
    }
    public List<EnhanceData> GetEnhance(int count)
    {
        return _enhanceDatas.ShuffleData().Take(count).ToList();
    }
}
