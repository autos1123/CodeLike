using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD:UIBase
{
    private PlayerController player;
    private PlayerActiveItemController activeItemController;

    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] Image HPFill;
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


        activeItemController.OnActiveItemCoolTime[1] -= ChangeItemSlot1CoolTime;

        activeItemController.OnActiveItemCoolTime[2] -= ChangeItemSlot2CoolTime;
    }
    //07_15 : 만약 플레이어 객체가 UI보다 먼저 파괴될 경우 Close()가 호출되지않을수 있어서
    //스크립트 파괴시 구독해제 추가
    private void OnDestroy()
    {
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
        HPFill.fillAmount = player.Condition.GetConditionRatio(ConditionType.HP);
    }

    void ChangeItemSlot1CoolTime(float time)
    {
        ItemSlot1CoolTime.fillAmount = time;
    }
    void ChangeItemSlot2CoolTime(float time)
    {
        ItemSlot2CoolTime.fillAmount = time;
    }

    public void UpdateActiveItemIcons()
    {
        // 0번 슬롯
        if(activeItemController != null && activeItemController.activeItemDatas.Count > 0 && activeItemController.activeItemDatas[0] != null)
        {
            ItemSlot1.sprite = Resources.Load<Sprite>(activeItemController.activeItemDatas[0].IconPath);
        }
        else
            ItemSlot1.sprite = null;

        // 1번 슬롯
        if(activeItemController != null && activeItemController.activeItemDatas.Count > 1 && activeItemController.activeItemDatas[1] != null)
        {
            ItemSlot2.sprite = Resources.Load<Sprite>(activeItemController.activeItemDatas[1].IconPath);
        }
        else
            ItemSlot2.sprite = null;
    }

}

