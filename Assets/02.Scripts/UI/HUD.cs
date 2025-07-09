using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD:UIBase
{
    private PlayerController player;

    [SerializeField] Button minMapButton;
    [SerializeField] Button optionButton;
    [SerializeField] Button minimapButton;
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

        player.Condition.statModifiers[ConditionType.Gold] += ChangeGold;
        ChangeGold();

        player.Condition.statModifiers[ConditionType.HP] += ChangeHP;
        ChangeHP();

        minimapButton.onClick.RemoveAllListeners();
        minimapButton.onClick.AddListener(() =>
        {
            UIManager.Instance.ToggleUI<MinimapUI>();
        });
    }
    public override void Close()
    {
        base.Close();
        if(player == null) return;

        player.Condition.statModifiers[ConditionType.Gold] -= ChangeGold;
        player.Condition.statModifiers[ConditionType.HP] -= ChangeHP;
    }

    void ChangeGold()
    {
        goldText.text = player.Condition.GetValue(ConditionType.Gold).ToString();
    }

    void ChangeHP()
    {
        float currentHP = player.Condition.GetValue(ConditionType.HP);

        if(player.Condition.Data.TryGetCondition(ConditionType.HP, out float maxHP))
        {
            HPFill.fillAmount = currentHP / maxHP;
        }
        else
        {
            Debug.LogError("[HUD] Condition에서 maxHP를 받아올 수 없습니다.");
        }
    }
}
