using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoSingleton<ItemManager>
{

    TableManager _tableManager;
    List<ItemData> _itemDataDats;

    protected override bool Persistent => false;
    public void Init(TableManager tableManager)
    {
        _tableManager = tableManager;
        _itemDataDats = tableManager.GetTable<ItemDataTable>().dataList;
    }
}
