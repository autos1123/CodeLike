using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 상점 전용 인벤토리. 초기 아이템 셋업 및 IInventory 인터
/// </summary>
public class ShopInventory : MonoBehaviour,IInventory
{
    public int npcID;
    
    private ItemDataTable itemDataTable;
    private NPCDataTable npcDataTable;
    
    public List<InventoryItemSlot> inventorySlots = new List<InventoryItemSlot>();
    public bool Initialized { get; private set; } = false;
    public event Action OnInitialized;
    
    private void Awake()
    {
        if (TryGetComponent<NPCController>(out var npcController))
        {
            npcID = npcController.ID;
        }
        else
        {
            Debug.LogWarning("[ShopInventory] NPCController를 찾을 수 없습니다.");
        }
    }
    /// <summary>
    /// 테이블 로드가 완료될 때까지 대기 후 상점 아이템 초기화
    /// </summary>
    private void Start()
    {
        itemDataTable = TableManager.Instance.GetTable<ItemDataTable>();
        npcDataTable = TableManager.Instance.GetTable<NPCDataTable>();
        
        Init();
        var npcData = npcDataTable.GetDataByID(npcID);
        if (npcData == null)
        {
            Debug.LogError($"[ShopInventory] NPCData를 찾을 수 없습니다. ID: {npcID}");
            return;
        }

        foreach (var id in npcData.shopItemIDs)
        {
            var item = itemDataTable.GetDataByID(id);
            if (item != null)
                inventorySlots.Add(CreateSlot(item));
        }
        // var item1 = itemDataTable.GetDataByID(6000);
        // var item2 = itemDataTable.GetDataByID(6001);
        // inventorySlots.Add(CreateSlot(item1));
        // inventorySlots.Add(CreateSlot(item2));
        Initialized = true;
        OnInitialized?.Invoke();
    }

    /// <summary>
    /// 상점 인벤토리 초기화 (슬롯 클리어)
    /// </summary>
    public void Init()
    {
        inventorySlots.Clear();
    }
    
    /// <summary>
    /// 지정된 아이템과 수량으로 새로운 ItemSlot 생성
    /// </summary>
    private InventoryItemSlot CreateSlot(ItemData item)
    {
        var slot = new InventoryItemSlot();
        slot.Set(item);
        return slot;
    }
    
    /// <summary>
    /// 현재 보유한 모든 아이템 슬롯 반환 (IInventory 구현)
    /// </summary>
    public List<InventoryItemSlot> GetInventorySlots(bool includeEquip = false) => inventorySlots;

    /// <summary>
    /// 아이템을 상점 인벤토리에 추가 (빈 슬롯에만 추가)
    /// </summary>
    public bool AddToInventory(ItemData item)
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.IsEmpty)
            {
                slot.Set(item);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 아이템을 상점 인벤토리에서 제거
    /// </summary>
    public bool RemoveFromInventory(ItemData item)
    {
        foreach (var slot in inventorySlots)
        {
            if (!slot.IsEmpty && slot.InventoryItem == item)
            {
                slot.Clear();
                return true;
            }
        }
        return false;
    }

}
