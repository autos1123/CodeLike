[System.Serializable]

/// <summary>
/// 하나의 인벤토리 슬롯에 해당하는 데이터 구조.
/// 아이템과 수량 정보를 포함.
/// </summary>
public class ItemSlot
{
    /// <summary> 슬롯에 담긴 아이템 데이터 </summary>
    public ItemData Item { get; private set; }
    
    /// <summary> 아이템 수량 </summary>
    public int Quantity { get; private set; }

    /// <summary> 슬롯이 비어있는지 여부 </summary>
    public bool IsEmpty => Item == null;

    /// <summary>
    /// 슬롯에 아이템과 수량을 설정
    /// </summary>
    public void Set(ItemData item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }
    
    /// <summary>
    /// 슬롯을 비움
    /// </summary>
    public void Clear()
    {
        Item = null;
        Quantity = 0;
    }
    
    /// <summary>
    /// 현재 슬롯의 아이템과 수량을 복제한 새로운 슬롯 반환
    /// </summary>
    /// <returns>복제된 ItemSlot</returns>
    public ItemSlot Clone()
    {
        var clone = new ItemSlot();
        if (!IsEmpty)
            clone.Set(Item, Quantity);
        return clone;
    }

    /// <summary>
    /// 슬롯의 아이템이 특정 아이템과 동일한지 비교
    /// </summary>
    /// <param name="target">비교할 아이템</param>
    /// <returns>동일 여부</returns>
    public bool IsSameItem(ItemData target)
    {
        return !IsEmpty && Item.ID == target.ID;
    }

    /// <summary>
    /// 현재 슬롯에 있는 아이템의 설명 반환 (툴팁 등에서 사용)
    /// </summary>
    public string GetDescription()
    {
        return IsEmpty ? "" : Item.description;
    }
}
