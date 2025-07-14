using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DestinyBoard:UIBase
{
    [SerializeField] DestinyCard[] cards;
    [SerializeField] Button FailedButton;
    TableManager _tableManager;
    public override string UIName => "DestinyBoard";


    void Awake()
    {
        _tableManager = TableManager.Instance;
    }
    
    public override void Open()
    {
        base.Open();
        if(_tableManager == null) _tableManager = TableManager.Instance;
        var destinys = _tableManager.GetTable<DestinyDataTable>().dataList.ShuffleData().Take(cards.Count()).ToArray();        
        for(int i = 0; i < cards.Length; i++)
        {
            cards[i].init(destinys[i]);
        }

        FailedButton.onClick.RemoveAllListeners();
        FailedButton.onClick.AddListener(ClickFailed);
    }

    public override void Close()
    {
        foreach(var item in cards)
        {
            item.Clear();
        }        
        base.Close();
    }


    void ClickFailed()
    {
        gameObject.SetActive(false);
    }
}
