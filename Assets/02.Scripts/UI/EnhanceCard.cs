using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EnhanceCard : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _enhaceDescription;

    private UIBase _board;
    private EnhanceData _enhanceData;

    void Start()
    {
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(Click);
    }

    public void init(EnhanceData enhanceData , UIBase uIBase)
    {
        _board = uIBase;
        _enhanceData = enhanceData;
        _title.text = enhanceData.name;
        _enhaceDescription.text = enhanceData.dsecription;
    }
    public void Clear()
    {
        _title.text = string.Empty;
        _enhaceDescription.text = string.Empty;
    }
    void Click()
    {
        if(GameManager.Instance.Player.TryGetComponent<PlayerController>(out var playerController))
        {
            playerController.Condition.ChangeModifierValue(_enhanceData.ConditionType, ModifierType.BuffEnhance, _enhanceData.value);
            //todo : 강화 수치에 대핸 데이터를 넘겨준다.
        }
        _board.Close();
    }

}
