using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestinyManager : MonoSingleton<DestinyManager>
{
    TableManager _tableManager;
    [SerializeField]List<DestinyData> _destinyDatas;
    protected override bool Persistent => false;
    public void Init(TableManager tableManager)
    {
        _tableManager = tableManager;
        tableManager.loadComplet += getData;
    }

    public void getData()
    {
        _destinyDatas = _tableManager.GetTable<DestinyDataTable>().dataList;
    }
    public List<DestinyData> GetDestinys(int count)
    {
        return _destinyDatas.ShuffleData().Take(count).ToList();
    }
}
