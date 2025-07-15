using System;
using System.Linq;
using UnityEngine;


public class EnhanceBoard : UIBase
{
    [SerializeField] EnhanceCard[] cards;
    TableManager _tableManager;
    public override string UIName => "EnhanceBoard";

    public override void Open()
    {
        base.Open();
        if(_tableManager == null) _tableManager = TableManager.Instance;
        var enhance = _tableManager.GetTable<EnhanceDataTable>().dataList.ShuffleData().Take(cards.Count()).ToArray();

        for(int i = 0; i < cards.Length; i++)
        {
            cards[i].init(enhance[i],this);
        }
    }
    public override void Close()
    {
        foreach(var item in cards)
        {
            item.Clear();
        }
        base.Close();
    }

}
