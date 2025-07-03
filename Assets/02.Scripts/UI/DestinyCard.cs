using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DestinyCard : MonoBehaviour
{
    private Button _button;

    private TextMeshProUGUI _title;
    private TextMeshProUGUI _positiveDescription;
    private TextMeshProUGUI _negativeDescription;

    private TableManager _tableManager;
    private UIBase _board;
    [SerializeField]private DestinyData _destinyData;

    void Awake()
    {
        _button = GetComponent<Button>();
        _title = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        _positiveDescription = transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        _negativeDescription = transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        _board = transform.parent.GetComponent<UIBase>();

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
