using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnhanceBoard : UIBase
{
    EnhanceCard[] cards;
    EnhanceManager _enhanceManager;

    public override string UIName => "enhanceBoard";

    private void Awake()
    {
        cards = transform.GetComponentsInChildren<EnhanceCard>();
    }

    public void OnEnable()
    {
        
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
