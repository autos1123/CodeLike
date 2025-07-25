using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EnhanceCard:MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _enhaceDescription;

    private UIBase _board;
    private EnhanceData _enhanceData;
    private float _randomIncreaseValue = 0f;

    void Start()
    {
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(Click);
    }

    public void init(EnhanceData enhanceData, UIBase uIBase)
    {
        _board = uIBase;
        _enhanceData = enhanceData;
        _title.text = enhanceData.name;

        float value = Random.Range(enhanceData.minvalue, enhanceData.maxvalue);

        // 1. 플레이어 기본 ConditionData에서 "초기값" 가져오기
        var playerConditionData = TableManager.Instance.GetTable<ConditionDataTable>().dataList
            .FirstOrDefault(cd => cd.CharacterName == "Player");
        float baseStat = 0f;
        if(playerConditionData != null)
        {
            playerConditionData.InitConditionDictionary();
            playerConditionData.TryGetCondition(_enhanceData.ConditionType, out baseStat);
        }

        // 2. 플레이어 현재 Condition 값 가져오기
        float currentStat = 0f;
        if(GameManager.Instance.Player.TryGetComponent<PlayerController>(out var playerController))
        {
            currentStat = playerController.Condition.GetValue(_enhanceData.ConditionType);
        }

        // 3. 증가량/설명
        float increaseStat = baseStat * value; // 기준은 "초기값" 배율 (원하는대로 바꿔도 됨)
        float finalStat = currentStat + increaseStat;

        string statName = _enhanceData.name.Replace("증가", "");
        string line1 = $"{statName}이 {increaseStat:F1} 증가합니다.";
        string line2 = $"강화 후: {currentStat:F2} → {finalStat:F1}";
        _enhaceDescription.text = line1 + "\n" + line2;

        _randomIncreaseValue = increaseStat;
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
            playerController.Condition.ChangeModifierValue(_enhanceData.ConditionType, ModifierType.BuffEnhance, _randomIncreaseValue);
        }
        _board.Close();
    }
}
