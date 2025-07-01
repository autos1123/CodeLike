using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnhanceManager :MonoSingleton<EnhanceManager>
{
    TableManager _tableManager;
    [SerializeField] List<EnhanceData> _enhanceDatas;
    protected override bool Persistent => false;
    public void Init(TableManager tableManager)
    {
        _tableManager = tableManager;
        tableManager.loadComplet += getData;
    }
    public void getData()
    {
        _enhanceDatas = _tableManager.GetTable<EnhanceDataTable>().dataList;
    }
    public List<EnhanceData> GetEnhance(int count)
    {
        return _enhanceDatas.ShuffleData().Take(count).ToList();
    }   

}
