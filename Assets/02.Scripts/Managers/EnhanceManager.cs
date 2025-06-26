using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnhanceManager :MonoSingleton<EnhanceManager>
{
    TableManager _tableManager;
    List<EnhanceData> enhanceDatadatas;
    public void Init(TableManager tableManager)
    {
        _tableManager = tableManager;
        enhanceDatadatas = _tableManager.GetTable<EnhanceDataTable>().dataList;
    }
    public List<EnhanceData> GetEnhance(int count)
    {
        return enhanceDatadatas.ShuffleData().Take(count).ToList();
    }
}
