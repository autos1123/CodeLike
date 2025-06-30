using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DestinyBoard : MonoBehaviour
{
    DestinyCard[] cards;
    DestinyManager _destinyManager;

    /// <summary>
    /// 실행히 
    /// </summary>
    public void OnEnable()
    {
        cards = transform.GetComponentsInChildren<DestinyCard>();
        _destinyManager = BattleCoreManager.Instance.DestinyManager;
        var destinys = _destinyManager.GetDestinys(cards.Count());

        for(int i = 0; i < cards.Length; i++)
        {
            cards[i].init(destinys[i]);
        }
    }
}
