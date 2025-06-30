using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DestinyBoard : MonoBehaviour
{
    DestinyCard[] cards;
    DestinyManager _destinyManager;
    Button FailedButton;

    public void Awake()
    {
        FailedButton = transform.GetChild(3).GetComponent<Button>();
    }
    public void OnEnable()
    {
        cards = transform.GetComponentsInChildren<DestinyCard>();
        _destinyManager = BattleCoreManager.Instance.DestinyManager;
        var destinys = _destinyManager.GetDestinys(cards.Count());

        for(int i = 0; i < cards.Length; i++)
        {
            cards[i].init(destinys[i]);
        }

        FailedButton.onClick.RemoveAllListeners();
        FailedButton.onClick.AddListener(ClickFailed);
    }

    public void OnDisable()
    {
        foreach(var item in cards)
        {
            item.Clear();
        }
        cards=null;
    }

    void ClickFailed()
    {
        Debug.Log("거부눌림");
        gameObject.SetActive(false);
    }
}
