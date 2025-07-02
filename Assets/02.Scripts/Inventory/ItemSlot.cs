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
}
