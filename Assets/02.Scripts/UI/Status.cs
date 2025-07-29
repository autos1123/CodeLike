using TMPro;
using UnityEngine;

public class Status : MonoBehaviour
{
    TextMeshProUGUI text;
    [SerializeField] ConditionType _conditionType;

    public void init(ConditionType conditionType)
    {
        _conditionType = conditionType;
    }

    private void OnEnable()
    {
        if (text == null)
            text = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        var gameManager = GameManager.Instance;
        if (gameManager == null || gameManager.Player == null) return;

        var playerController = gameManager.Player.GetComponent<PlayerController>();
        if (playerController == null || playerController.Condition == null) return;

        if (!playerController.Condition.statModifiers.ContainsKey(_conditionType)) return;

        playerController.Condition.statModifiers[_conditionType] += onChangeText;
        onChangeText();
    }

    private void OnDisable()
    {
        var gameManager = GameManager.Instance;
        if (gameManager == null || gameManager.Player == null) return;

        var playerController = gameManager.Player.GetComponent<PlayerController>();
        if (playerController == null || playerController.Condition == null) return;

        if (!playerController.Condition.statModifiers.ContainsKey(_conditionType)) return;

        playerController.Condition.statModifiers[_conditionType] -= onChangeText;
    }

    void onChangeText()
    {
        text.text = GameManager.Instance.Player.GetComponent<PlayerController>().Condition.GetStatus(_conditionType);
    }
}
