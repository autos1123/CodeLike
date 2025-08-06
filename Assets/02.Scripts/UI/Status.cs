using TMPro;
using UnityEngine;

public class Status:MonoBehaviour
{
    TextMeshProUGUI text;
    [SerializeField] ConditionType _conditionType;

    public void init(ConditionType conditionType)
    {
        _conditionType = conditionType;
    }

    private void OnEnable()
    {
        if(text == null)
            text = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        var gameManager = GameManager.Instance;
        if(gameManager == null || gameManager.Player == null) return;

        var playerController = gameManager.Player.GetComponent<PlayerController>();
        if(playerController == null || playerController.Condition == null) return;

        if(!playerController.Condition.statModifiers.ContainsKey(_conditionType)) return;

        playerController.Condition.statModifiers[_conditionType] += onChangeText;
        onChangeText();
    }

    private void OnDisable()
    {
        var gameManager = GameManager.Instance;
        if(gameManager == null || gameManager.Player == null) return;

        var playerController = gameManager.Player.GetComponent<PlayerController>();
        if(playerController == null || playerController.Condition == null) return;

        if(!playerController.Condition.statModifiers.ContainsKey(_conditionType)) return;

        playerController.Condition.statModifiers[_conditionType] -= onChangeText;
    }

    void onChangeText()
    {
        var player = GameManager.Instance.Player.GetComponent<PlayerController>();
        var condition = player.Condition;
        
        float baseValue = condition.GetOriginConditionValue(_conditionType);
        float itemEnhance = condition.GetModifierValue(ModifierType.ItemEnhance, _conditionType);
        float buffEnhance = condition.GetModifierValue(ModifierType.BuffEnhance, _conditionType);
        float enhanceValue = itemEnhance + buffEnhance;
        
        float currentValue = player.Condition.GetCurrentConditionValue(_conditionType);
        
        float totalMaxValue = baseValue + enhanceValue;
        
        float maxValue = player.Condition.GetTotalMaxValue(_conditionType);
        if (_conditionType == ConditionType.HP)
        {
            currentValue = player.Condition.CurrentConditions[_conditionType];
            text.text = $"{currentValue:F0} / {totalMaxValue:F0} <size=50%>({baseValue:F0} + {enhanceValue:F0})</size>";
        }
        else if(_conditionType == ConditionType.Stamina)
        {
            text.text = $"{totalMaxValue:F0} <size=50%>({baseValue:F0} + {enhanceValue:F0})</size>";
        }
        else if (_conditionType == ConditionType.CriticalChance || _conditionType == ConditionType.CriticalDamage)
        {
            float basePercent = baseValue * 100f;
            float enhancePercent = enhanceValue * 100f;
            float totalPercent = basePercent + enhancePercent;
            
            text.text = $"{totalPercent:F0}% <size=50%>({basePercent:F0}% + {enhancePercent:F0}%)</size>";
        }
        else
        {
            text.text = $"{totalMaxValue:F2} <size=50%>({baseValue:F2} + {enhanceValue:F2})</size>";
        }
    }
}
