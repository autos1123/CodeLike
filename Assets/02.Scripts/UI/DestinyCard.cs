using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DestinyCard : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _positiveDescription;
    [SerializeField] private TextMeshProUGUI _negativeDescription;

    private TableManager _tableManager;
    private UIBase _board;
    private DestinyData _destinyData;

    void Awake()
    {
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(Click);
    }

    public void init(DestinyData destinyData)
    {
        if(_tableManager == null) _tableManager = TableManager.Instance;

        _destinyData = destinyData;
        _title.text = _destinyData.Name;
        _positiveDescription.text = _tableManager.GetTable<DestinyEffectDataTable>().GetDataByID(_destinyData.NegativeEffectDataID).dsecription;
        _negativeDescription.text = _tableManager.GetTable<DestinyEffectDataTable>().GetDataByID(_destinyData.PositiveEffectDataID).dsecription;        
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
        GameManager.Instance.setCurDestinyData(_destinyData);
        _board.Close();
    }

}
