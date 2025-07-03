using System.Linq;


public class EnhanceBoard : UIBase
{
    EnhanceCard[] cards;
    TableManager _tableManager;
    public override string UIName => "EnhanceBoard";

    private void Awake()
    {
        cards = transform.GetComponentsInChildren<EnhanceCard>();
        _tableManager = TableManager.Instance;
    }

    public override void Open()
    {
        base.Open();

        var enhance = _tableManager.GetTable<EnhanceDataTable>().dataList.ShuffleData().Take(cards.Count()).ToArray();

        for(int i = 0; i < cards.Length; i++)
        {
            cards[i].init(enhance[i]);
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
