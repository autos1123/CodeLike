using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DestinyCard : MonoBehaviour
{
    private Button _button;

    private TextMeshProUGUI _title;
    private TextMeshProUGUI _positiveDescription;
    private TextMeshProUGUI _negativeDescription;

    private BattleCoreManager _battleCoreManager;
    [SerializeField]private DestinyData _destinyData;
    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponent<Button>();
        _title = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        _positiveDescription = transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        _negativeDescription = transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        _battleCoreManager = BattleCoreManager.Instance;

        _button.onClick.AddListener(Click);

    }

    public void init(DestinyData destinyData)
    {
        _destinyData = destinyData;
        _title.text = _destinyData.Name;
        _positiveDescription.text = _destinyData.PositiveDescription;
        _negativeDescription.text = _destinyData.NegativeDescription;
    }
    public void Clear()
    {
        _destinyData = null;
        _title.text = string.Empty;
        _positiveDescription.text = string.Empty;
        _negativeDescription.text = string.Empty;
    }
    void Click()
    {
        Debug.Log("눌림");
        _battleCoreManager.setCurDestinyData(_destinyData);
    }

    
}
