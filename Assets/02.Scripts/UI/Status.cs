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

        float currentValue = player.Condition.GetCurrentConditionValue(_conditionType);
        float maxValue = player.Condition.GetTotalMaxValue(_conditionType);
        if (_conditionType == ConditionType.HP)
        {
            currentValue = player.Condition.CurrentConditions[_conditionType];
            text.text = $"{currentValue:F0} / {maxValue:F0}";
        }
        else if(_conditionType == ConditionType.Stamina)
        {
            text.text = $"{maxValue:F0}";
        }
        else
        {
            text.text = currentValue.ToString("F2");
        }
    }
}
