using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public interface IInventory
{
    bool Initialized { get; }
    event Action OnInitialized;
    List<InventoryItemSlot> GetInventorySlots(bool includeEquip = false); // 장비슬롯도 같이 가져올수있게

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

    public List<InventoryItemSlot> inventorySlots = new List<InventoryItemSlot>();
    /// <summary> 실제 장비 슬롯 리스트 (4칸) </summary>
    
    public List<InventoryItemSlot> equipSlots = new List<InventoryItemSlot>();
    
    public List<ActiveItemSlot> activeItemSlots = new List<ActiveItemSlot>();
    
    /// <summary> 인벤토리가 초기화 완료되었는지 여부 </summary>
    public bool Initialized { get; private set; } = false;
    public event Action OnInitialized;
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
        
        // // 테스트 아이템 추가 (인벤토리슬로ㅓㅅ)
        // var item_1 = itemDataTable.GetDataByID(6000);
        // AddToInventory(item_1);
        // var item_2 = itemDataTable.GetDataByID(6001);
        // AddToInventory(item_2);
        // var item_3 = itemDataTable.GetDataByID(6000);
        // AddToInventory(item_3);
        //
        // //// 테스트 아이템 추가 (액티브아이템 슬롯)
        // //var item_4 = activeItemDataTable.GetDataByID(4000);
        // //AddtoActiveSlot(0, item_4, Skillinput.X);
        // //var item_5 = activeItemDataTable.GetDataByID(4001);
        // //AddtoActiveSlot(1, item_5, Skillinput.C);
        
        
        
        Initialized = true;
        OnInitialized?.Invoke();
        
        UIManager.Instance.ShowUI<HUD>();

    }
    
    /// <summary>
    /// 인벤토리 슬롯을 초기화하고 비워진 상태로 설정
    /// </summary>
    public void Init()
    {
        inventorySlots.Clear();
        for (int i = 0; i < 16; i++)
            inventorySlots.Add(new InventoryItemSlot());
        
        equipSlots.Clear();
        for (int i = 0; i < 4; i++)
            equipSlots.Add(new InventoryItemSlot());
        
        activeItemSlots.Clear();
        for(int i = 0; i<2; i++)
            activeItemSlots.Add(new ActiveItemSlot());
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
            if (slot.IsEmpty)
            {
                slot.Set(item);
                return true;
            }
        }
        UIManager.Instance.ShowConfirmPopup(
            "인벤토리가 가득 차서 아이템을 추가할 수 없습니다.",
            onConfirm: () => { },
            onCancel: null,
            confirmText: "확인"
        );
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
            if (slot.IsEmpty)
            {
                slot.Set(activeItem);
                PlayerActiveItemController.TakeItem(activeItem);
                return true;
            }
        }
        UIManager.Instance.ShowConfirmPopup(
            "액티브아이템 슬롯이 가득 차서 아이템을 추가할 수 없습니다.",
            onConfirm: () => { },
            onCancel: null,
            confirmText: "확인"
        );
        return false;
    }
    /// <summary>
    /// 선택한 인덱스의 액티브아이템 슬롯에(비어있을 때만) 아이템 추가
    /// </summary>
    /// <param name="slotIndex">장착할 슬롯 인덱스</param>
    /// <param name="activeItem">추가할 아이템</param>
    /// <param name="skillinput">스킬 입력</param>
    /// <returns>성공 시 true, 실패 시 false</returns>
    public bool AddtoActiveSlot(int slotIndex, ActiveItemData activeItem, Skillinput skillinput)
    {
        // 범위 체크
        if(slotIndex < 0 || slotIndex >= activeItemSlots.Count)
            return false;

        var slot = activeItemSlots[slotIndex];
        if(slot.IsEmpty)
        {
            slot.Set(activeItem);
            PlayerActiveItemController.TakeItem(skillinput, activeItem);
            return true;
        }
        return false; // 이미 아이템이 들어있으면 추가 불가
    }

    public void ReplaceActiveSlot(int slotIndex, ActiveItemData newItem, Skillinput skillinput)
    {
        if(slotIndex < 0 || slotIndex >= activeItemSlots.Count)
            return; // 범위 오류 방지

        // 기존 아이템 정보
        var oldItem = activeItemSlots[slotIndex].ActiveItem;

        // 새 아이템 세팅(덮어쓰기)
        activeItemSlots[slotIndex].Set(newItem);

        // 컨트롤러에도 반영
        if(skillinput != null)
            PlayerActiveItemController.TakeItem(skillinput, newItem);
        else
            PlayerActiveItemController.TakeItem(newItem);

        // (옵션) oldItem을 인벤토리로 돌려주거나, 파기 등 처리 가능
    }

    /// <summary>
    /// 인벤토리 슬롯 반환
    /// </summary>
    /// <returns></returns>
    public List<InventoryItemSlot> GetInventorySlots(bool includeEquip = false)
    {
        var result = new List<InventoryItemSlot>(inventorySlots);
    
        if (includeEquip)
            result.AddRange(equipSlots);

        return result;
    }
    
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
