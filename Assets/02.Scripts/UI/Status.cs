using System.Collections;
using System.Collections.Generic;
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
        if(text == null) text = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        GameManager.Instance.Player.GetComponent<PlayerController>().PlayerCondition.statModifiers[_conditionType] += onChangeText;
        onChangeText();
    }

    private void OnDisable()
    {
        GameManager.Instance.Player.GetComponent<PlayerController>().PlayerCondition.statModifiers[_conditionType] -= onChangeText;
    }

    void onChangeText()
    {
        text.text = GameManager.Instance.Player.GetComponent<PlayerController>().PlayerCondition.GetValue(_conditionType).ToString();
    }
}
