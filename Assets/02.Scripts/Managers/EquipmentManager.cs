
/// <summary>
/// 장비 아이템 교체 및 능력치 적용을 담당하는 매니저 클래스.
/// 플레이어 상태(Stat)에 아이템 효과를 적용하거나 제거한다.
/// </summary>
public class EquipmentManager : MonoSingleton<EquipmentManager>
{
    private PlayerCondition playerCondition;

    private void Awake()
    {
        if (GameManager.Instance.Player.TryGetComponent<PlayerController>(out var controller))
        {
            playerCondition = controller.PlayerCondition;
        }
    }
    
    /// <summary>
    /// 아이템 슬롯 간 교체(Swap)를 수행하고, 기존 능력치를 제거한 뒤 교체된 아이템의 능력치를 다시 적용한다.
    /// </summary>
    /// <param name="slotA">첫 번째 슬롯</param>
    /// <param name="typeA">첫 번째 슬롯 타입</param>
    /// <param name="slotB">두 번째 슬롯</param>
    /// <param name="typeB">두 번째 슬롯 타입</param>
    public void SwapItemEffects(ItemSlot slotA, SlotType typeA, ItemSlot slotB, SlotType typeB)
    {
        if (playerCondition == null) return;
        if(typeA == SlotType.ActiveItem || typeB == SlotType.ActiveItem) return;
        // 기존 스탯 제거
        RemoveItemStat(slotA.Item, typeA);
        RemoveItemStat(slotB.Item, typeB);

        // 스왑
        var tempItem = slotA.Item;
        slotA.Set(slotB.Item);
        slotB.Set(tempItem);

        // 스탯 재적용
        ApplyItemStat(slotA.Item, typeA);
        ApplyItemStat(slotB.Item, typeB);
    }
    
    /// <summary>
    /// 아이템 효과를 플레이어 능력치에 적용한다. (장비 슬롯일 경우에만 적용)
    /// </summary>
    /// <param name="item">적용할 아이템</param>
    /// <param name="type">해당 슬롯 타입</param>
    private void ApplyItemStat(ItemData item, SlotType type)
    {
        if (item == null || type != SlotType.Equip) return;
        playerCondition.ChangeModifierValue(item.ConditionType, ModifierType.ItemEnhance, item.value);
    }
    
    /// <summary>
    /// 아이템 효과를 플레이어 능력치에서 제거한다. (장비 슬롯일 경우에만 제거)
    /// </summary>
    /// <param name="item">제거할 아이템</param>
    /// <param name="type">해당 슬롯 타입</param>
    private void RemoveItemStat(ItemData item, SlotType type)
    {
        if (item == null || type != SlotType.Equip) return;
        playerCondition.ChangeModifierValue(item.ConditionType, ModifierType.ItemEnhance, -item.value);
    }
}
