using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class HUD:UIBase
{
    private PlayerController player;
    private PlayerActiveItemController activeItemController;

    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] Image HPFill;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] Image StaminaFill;
    [SerializeField] private TextMeshProUGUI staminaText;
    [SerializeField] Image ItemSlot1;
    [SerializeField] Image ItemSlot1CoolTime;
    [SerializeField] Image ItemSlot2;
    [SerializeField] Image ItemSlot2CoolTime;
    public override string UIName => this.GetType().Name;

    public override void Open()
    {
        base.Open();
        player = GameManager.Instance.Player.GetComponent<PlayerController>();
        activeItemController = GameManager.Instance.Player.GetComponent<PlayerActiveItemController>();

        player.Condition.statModifiers[ConditionType.Gold] += ChangeGold;
        ChangeGold();

        player.Condition.statModifiers[ConditionType.HP] += ChangeHP;
        ChangeHP();

        player.Condition.statModifiers[ConditionType.Stamina] += ChangeStamina;
        ChangeStamina();

        // 아이템 슬롯 쿨타임 (Skillinput Enum 인덱스 사용)
        if(activeItemController.OnActiveItemCoolTime.Count < 1)
            activeItemController.OnActiveItemCoolTime.Add((float n) => { });
        activeItemController.OnActiveItemCoolTime[(int)Skillinput.X] += ChangeItemSlot1CoolTime;

        if(activeItemController.OnActiveItemCoolTime.Count < 2)
            activeItemController.OnActiveItemCoolTime.Add((float n) => { });
        activeItemController.OnActiveItemCoolTime[(int)Skillinput.C] += ChangeItemSlot2CoolTime;

        UpdateActiveItemIcons();
    }

    public override void Close()
    {
        base.Close();
        if(player == null) return;

        player.Condition.statModifiers[ConditionType.Gold] -= ChangeGold;
        player.Condition.statModifiers[ConditionType.HP] -= ChangeHP;
        player.Condition.statModifiers[ConditionType.Stamina] -= ChangeStamina;

        if(activeItemController.OnActiveItemCoolTime.Count > 1)
            activeItemController.OnActiveItemCoolTime[(int)Skillinput.X] -= ChangeItemSlot1CoolTime;
        if(activeItemController.OnActiveItemCoolTime.Count > 2)
            activeItemController.OnActiveItemCoolTime[(int)Skillinput.C] -= ChangeItemSlot2CoolTime;
    }

    private void OnDestroy()
    {
        var playerObject = GameManager.Instance?.Player;
        if (playerObject != null)
        {
            if (playerObject.TryGetComponent<PlayerController>(out var controller))
            {
                controller.Condition.statModifiers[ConditionType.Gold] -= ChangeGold;
                controller.Condition.statModifiers[ConditionType.HP] -= ChangeHP;
                controller.Condition.statModifiers[ConditionType.Stamina] -= ChangeStamina;
            }
        }

        if(activeItemController.OnActiveItemCoolTime.Count > 1)
            activeItemController.OnActiveItemCoolTime[(int)Skillinput.X] -= ChangeItemSlot1CoolTime;
        if(activeItemController.OnActiveItemCoolTime.Count > 2)
            activeItemController.OnActiveItemCoolTime[(int)Skillinput.C] -= ChangeItemSlot2CoolTime;
    }

    void ChangeGold()
    {
        goldText.text = player.Condition.GetTotalCurrentValue(ConditionType.Gold).ToString();
    }

    void ChangeHP()
    {
        if (HPFill == null || player == null || player.Condition == null) return;

        float currentHP = player.Condition.CurrentConditions[ConditionType.HP]; // 실제 현재 HP
        float maxHP = player.Condition.GetTotalMaxValue(ConditionType.HP); // 강화 포함한 최대 HP
        float HPratio = currentHP / maxHP;

        HPFill.fillAmount = Mathf.Min(1f, currentHP / maxHP);
        
        if (hpText != null)
            hpText.text = $"{Mathf.FloorToInt(currentHP)} / {Mathf.FloorToInt(maxHP)}";

        if(HPratio <= 0.3f)
            hpText.color = Color.red;
        else
            hpText.color = Color.white;
    }

    void ChangeStamina()
    {
        if (StaminaFill == null || player == null || player.Condition == null) return;

        float currentStamina = player.Condition.CurrentConditions[ConditionType.Stamina]; // 실제 현재 HP
        float maxStamina = player.Condition.GetTotalMaxValue(ConditionType.Stamina); // 강화 포함한 최대 HP
        float ratio = currentStamina / maxStamina;

        StaminaFill.fillAmount = Mathf.Min(1f,(currentStamina / maxStamina));
        
        if (staminaText != null)
            staminaText.text = $"{Mathf.FloorToInt(currentStamina)} / {Mathf.FloorToInt(maxStamina)}";


        // 스태미나 비율이 30% 이하이면 붉게 표시
        if(ratio <= 0.3f)
            staminaText.color = Color.red;
        else
            staminaText.color = Color.white; 
    }

    void ChangeItemSlot1CoolTime(float time)
    {
        if (ItemSlot1CoolTime == null) return;
        ItemSlot1CoolTime.gameObject.SetActive(time > 0);
        ItemSlot1CoolTime.fillAmount = time;
    }

    void ChangeItemSlot2CoolTime(float time)
    {
        if (ItemSlot2CoolTime == null) return;
        ItemSlot2CoolTime.gameObject.SetActive(time > 0);
        ItemSlot2CoolTime.fillAmount = time;
    }

    public void UpdateActiveItemIcons()
    {
        // 0번 슬롯
        if(activeItemController != null && activeItemController.activeItemDatas.Count > 0 && activeItemController.activeItemDatas[0] != null)
        {
            ItemSlot1.gameObject.SetActive(true);
            ItemSlot1CoolTime.gameObject.SetActive(false);
            ItemSlot1.sprite = Resources.Load<Sprite>(activeItemController.activeItemDatas[0].IconPath);
        }
        else
        {
            ItemSlot1.gameObject.SetActive(false);
            ItemSlot1CoolTime.gameObject.SetActive(false);
            ItemSlot1.sprite = null;
        }

        // 1번 슬롯
        if(activeItemController != null && activeItemController.activeItemDatas.Count > 1 && activeItemController.activeItemDatas[1] != null)
        {
            ItemSlot2.gameObject.SetActive(true);
            ItemSlot2CoolTime.gameObject.SetActive(false);
            ItemSlot2.sprite = Resources.Load<Sprite>(activeItemController.activeItemDatas[1].IconPath);
        }
        else
        {
            ItemSlot2.gameObject.SetActive(false);
            ItemSlot2CoolTime.gameObject.SetActive(false);
            ItemSlot2.sprite = null;
        }
    }
}
