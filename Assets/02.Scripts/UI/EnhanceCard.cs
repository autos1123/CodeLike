using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class EnhanceCard : MonoBehaviour
{
    private Button _button;

    private TextMeshProUGUI _title;
    private TextMeshProUGUI _enhaceDescription;
    private BattleCoreManager _battleCoreManager;

    private EnhanceData _enhanceData;
    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponent<Button>();
        _title = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        _enhaceDescription = transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();

        _battleCoreManager = BattleCoreManager.Instance;

        _button.onClick.AddListener(Click);
    }

    public void init(EnhanceData enhanceData)
    {
        _enhanceData = enhanceData;
    }
    public void Clear()
    {
        _title.text = string.Empty;
        _enhaceDescription.text = string.Empty;
    }
    void Click()
    {
        if(BattleCoreManager.Instance.PlayerManager.Player.TryGetComponent<BaseCondition>(out var baseCondition))
        {
            //todo : 강화 수치에 대핸 데이터를 넘겨준다.
        }
    }

}
