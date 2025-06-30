using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnhanceBoard : MonoBehaviour
{
    EnhanceCard[] cards;
    EnhanceManager _enhanceManager;

    public void OnEnable()
    {
        cards = transform.GetComponentsInChildren<EnhanceCard>();
        _enhanceManager = BattleCoreManager.Instance.EnhanceManager;
        var enhance = _enhanceManager.GetEnhance(cards.Count());

        for(int i = 0; i < cards.Length; i++)
        {
            cards[i].init(enhance[i]);
        }

    }

    public void OnDisable()
    {
        foreach(var item in cards)
        {
            item.Clear();
        }
        cards = null;
    }
}
