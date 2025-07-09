using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public interface IInventory
{
    bool Initialized { get; }
    List<ItemSlot> GetInventorySlots(bool includeEquip = false); // 장비슬롯도 같이 가져올수있게

    bool AddToInventory(ItemData item);
    bool RemoveFromInventory(ItemData item); // 필요 시
}
/// <summary>
/// 인벤토리 데이터를 관리하는 클래스.
/// 슬롯 초기화 및 아이템 추가 기능을 포함.
/// </summary>
public class Inventory : MonoBehaviour, IInventory
{
    private ItemDataTable itemDataTable;
    private ActiveItemDataTable activeItemDataTable;
    private PlayerActiveItemController PlayerActiveItemController;
    /// <summary> 실제 인벤토리 슬롯 리스트 (16칸) </summary>

    public List<ItemSlot> inventorySlots = new List<ItemSlot>();
    /// <summary> 실제 장비 슬롯 리스트 (4칸) </summary>
    
    public List<ItemSlot> equipSlots = new List<ItemSlot>();
    
    public List<ItemSlot> activeItemSlots = new List<ItemSlot>();
    
    /// <summary> 인벤토리가 초기화 완료되었는지 여부 </summary>
    public bool Initialized { get; private set; } = false;
    
    public void InitializeInventory()
    {
        StartCoroutine(WaitAndInitialize());
    }
    
    /// <summary>
    /// 테이블 매니저가 로드 완료될 때까지 대기 후, 슬롯 초기화 및 테스트 아이템 추가
    /// </summary>
    private IEnumerator WaitAndInitialize()
    {
        yield return new WaitUntil(() => TableManager.Instance.loadComplete);
        itemDataTable = TableManager.Instance.GetTable<ItemDataTable>();
        activeItemDataTable = TableManager.Instance.GetTable<ActiveItemDataTable>();
        PlayerActiveItemController = transform.GetComponent<PlayerActiveItemController>();
        Init();
        
        // 테스트 아이템 추가 (인벤토리슬로ㅓㅅ)
        var item_1 = itemDataTable.GetDataByID(6000);
        AddToInventory(item_1);
        var item_2 = itemDataTable.GetDataByID(6001);
        AddToInventory(item_2);
        var item_3 = itemDataTable.GetDataByID(6000);
        AddToInventory(item_3);
        
        // 테스트 아이템 추가 (액티브아이템 슬롯)
        var item_4 = activeItemDataTable.GetDataByID(4000);
        AddtoActiveSlot(item_4);
        var item_5 = activeItemDataTable.GetDataByID(4001);
        AddtoActiveSlot(item_5);
        
        
        Initialized = true;

        
    }
    
    /// <summary>
    /// 인벤토리 슬롯을 초기화하고 비워진 상태로 설정
    /// </summary>
    public void Init()
    {
        inventorySlots.Clear();
        for (int i = 0; i < 16; i++)
            inventorySlots.Add(new ItemSlot());
        
        equipSlots.Clear();
        for (int i = 0; i < 4; i++)
            equipSlots.Add(new ItemSlot());
        
        activeItemSlots.Clear();
        for(int i = 0; i<2; i++)
            activeItemSlots.Add(new ItemSlot());
    }
    
    
    /// <summary>
    /// 비어 있는 슬롯에 아이템을 추가
    /// </summary>
    /// <param name="item">추가할 아이템</param>
    /// <returns>성공적으로 추가되었는지 여부</returns>
    public bool AddToInventory(ItemData item)
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.IsInvenSlotEmpty)
            {
                slot.Set(item);
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 비어있는 액티브아이템 슬롯에 아이템 추가
    /// </summary>
    /// <param name="activeItem">추가할 액티브아이템</param>
    /// <returns>성공적으로 추가되었는지 여부</returns>
    public bool AddtoActiveSlot(ActiveItemData activeItem)
    {
        foreach(var slot in activeItemSlots)
        {
            if (slot.IsActiveSlotEmpty)
            {
                slot.ActiveSlotSet(activeItem);
                PlayerActiveItemController.TakeItem(activeItem);
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 인벤토리 슬롯 반환
    /// </summary>
    /// <returns></returns>
    public List<ItemSlot> GetInventorySlots(bool includeEquip = false)
    {
        var result = new List<ItemSlot>(inventorySlots);
    
        if (includeEquip)
            result.AddRange(equipSlots);

        return result;
    }
    
    public bool RemoveFromInventory(ItemData item)
    {
        foreach (var slot in inventorySlots)
        {
            if (!slot.IsInvenSlotEmpty && slot.Item == item)
            {
                slot.Clear();
                return true;
            }
        }
        return false;
    }
    
}
