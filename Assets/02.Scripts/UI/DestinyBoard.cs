using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DestinyBoard : UIBase
{
    DestinyCard[] cards;
    DestinyManager _destinyManager;
    Button FailedButton;

    public override string UIName => "DestinyBoard";


    void Awake()
    {
        FailedButton = transform.GetChild(3).GetComponent<Button>();
        cards = transform.GetComponentsInChildren<DestinyCard>();
    }
    
    public override void Open()
    {
        base.Open();
        _destinyManager = BattleCoreManager.Instance.DestinyManager;
        var destinys = _destinyManager.GetDestinys(cards.Count());

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
        Debug.Log("거부눌림");
        gameObject.SetActive(false);
    }
}
