using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD:UIBase
{
    private PlayerController player;

    [SerializeField] Button minMapButton;
    [SerializeField] Button optionButton;
    [SerializeField] TextMeshProUGUI goldText;

    [SerializeField] Image HPFill;

    [SerializeField] Image ItemSlot1;
    [SerializeField] Image ItemSlot2;

    bool isOptionOpen = false;

    public override string UIName => "HUD";

    public override void Open()
    {
        base.Open();
        player = GameManager.Instance.Player.GetComponent<PlayerController>();

        optionButton.onClick.RemoveAllListeners();
        optionButton.onClick.AddListener(UIManager.Instance.ToggleUI<OptionBoard>);

        player.PlayerCondition.statModifiers[ConditionType.Gold] += ChangeGold;
        ChangeGold();

        player.PlayerCondition.statModifiers[ConditionType.HP] += ChangeHP;
        ChangeHP();
    }
    public override void Close()
    {
        base.Close();
        if(player == null) return;

        player.PlayerCondition.statModifiers[ConditionType.Gold] -= ChangeGold;
        player.PlayerCondition.statModifiers[ConditionType.HP] -= ChangeHP;
    }

    void ChangeGold()
    {

        goldText.text = player.PlayerCondition.GetValue(ConditionType.Gold).ToString();
        Debug.Log(goldText.text);
    }

    void ChangeHP()
    {
        HPFill.fillAmount = player.PlayerCondition.GetValue(ConditionType.HP) * 0.001f;
        Debug.Log(HPFill.fillAmount.ToString());
    }
}
