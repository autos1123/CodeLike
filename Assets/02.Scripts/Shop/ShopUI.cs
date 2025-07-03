using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopUI : UIBase
{
    public override string UIName => "ShopUI";

    [SerializeField] private Inventory playerInventoryRaw;
    [SerializeField] private ShopInventory shopInventoryRaw;
    private IInventory playerInventory => playerInventoryRaw;
    private IInventory shopInventory => shopInventoryRaw;

    public Transform sellParent;
    public Transform buyParent;
    public GameObject shopSlotPrefab;

    public TextMeshProUGUI sellTotalText;
    public TextMeshProUGUI buyTotalText;

    private List<ShopSlotUI> sellSlots = new List<ShopSlotUI>();
    private List<ShopSlotUI> buySlots = new List<ShopSlotUI>();
    
    public override void Open()
    {
        base.Open();
        if(playerInventoryRaw == null)
        {
            var inventoryUI = UIManager.Instance.GetUI<InventoryUI>();
            if(inventoryUI != null)
            {
                playerInventoryRaw = inventoryUI.GetComponent<Inventory>();
            }
        }
        if(shopInventoryRaw == null)
        {
            shopInventoryRaw = FindObjectOfType<ShopInventory>();
        }
        StartCoroutine(WaitForInventoryThenGenerate());
    }

    public override void Close()
    {
        base.Close();
        // 필요 시 슬롯 정리 등 추가 가능
    }

    private IEnumerator WaitForInventoryThenGenerate()
    {
        // 인벤토리 초기화 대기
        yield return new WaitUntil(() => 
            playerInventoryRaw != null && playerInventoryRaw.Initialized &&
            shopInventoryRaw != null && shopInventoryRaw.Initialized);
        
        GenerateSlots();
    }

    public void GenerateSlots()
    {
        foreach (Transform child in sellParent) Destroy(child.gameObject);
        foreach (Transform child in buyParent) Destroy(child.gameObject);
        sellSlots.Clear();
        buySlots.Clear();

        CreateSlots(playerInventory.GetInventorySlots(), sellParent, true, sellSlots);
        CreateSlots(shopInventory.GetInventorySlots(), buyParent, false, buySlots);
    }

    private void CreateSlots(List<ItemSlot> slots, Transform parent, bool isPlayer, List<ShopSlotUI> targetList)
    {
        foreach (var itemSlot in slots)
        {
            if (!itemSlot.IsEmpty)
            {
                var go = Instantiate(shopSlotPrefab, parent);
                var slotUI = go.GetComponent<ShopSlotUI>();
                slotUI.shopUI = this;
                slotUI.Set(itemSlot, isPlayer);
                targetList.Add(slotUI);
            }
        }
    }

    public void UpdateTotalPrices()
    {
        int sellTotal = 0;
        foreach (var slot in sellSlots)
        {
            if (slot.IsSelected && !slot.ItemSlot.IsEmpty)
                sellTotal += slot.ItemSlot.Item.sellPrice * slot.ItemSlot.Quantity;
        }

        int buyTotal = 0;
        foreach (var slot in buySlots)
        {
            if (slot.IsSelected && !slot.ItemSlot.IsEmpty)
                buyTotal += slot.ItemSlot.Item.buyPrice * slot.ItemSlot.Quantity;
        }

        sellTotalText.text = $"판매 금액: {sellTotal} G";
        buyTotalText.text = $"구매 금액: {buyTotal} G";
    }
}
