using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class DestinyManager : MonoSingleton<DestinyManager>
{
    TableManager _tableManager;
    List<DestinyData> _destinyDatas;
    DestinyBoard destinyBoard;
    protected override bool Persistent => false;

    private void Start()
    {
        destinyBoard =  GameObject.FindObjectOfType<DestinyBoard>();

    }
    public void Init(TableManager tableManager)
    {
        _tableManager = tableManager;
        _destinyDatas = tableManager.GetTable<DestinyDataTable>().dataList;

    }
    public List<DestinyData> GetDestinys(int count)
    {
        return _destinyDatas.ShuffleData().Take(count).ToList();
    }
}
