using System.Linq;
using UnityEngine;


public class EnhanceBoard : UIBase
{
    [SerializeField] EnhanceCard[] cards;
    TableManager _tableManager;
    public override string UIName => this.GetType().Name;

    public override void Open()
    {
        base.Open();
        if(_tableManager == null) _tableManager = TableManager.Instance;
        var enhanceDataList = _tableManager.GetTable<EnhanceDataTable>().dataList.ShuffleData().Take(cards.Count()).ToArray();

        for(int i = 0; i < cards.Length; i++)
        {
            cards[i].init(enhanceDataList[i],this);
        }
    }
    
    // 각 EnhanceCard에서 카드가 선택되었음을 알리는 메소드
    public void CardSelected(EnhanceCard selectcard)
    {
        foreach(var card in cards)
        {
            if(card != selectcard)
                card._selectButton.gameObject.SetActive(false);
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
