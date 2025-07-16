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
    [SerializeField] Image ItemSlot2;

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

        UpdateActiveItemIcons();
    }
    public override void Close()
    {
        base.Close();
        if(player == null) return;

        player.Condition.statModifiers[ConditionType.Gold] -= ChangeGold;
        player.Condition.statModifiers[ConditionType.HP] -= ChangeHP;
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

