using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public TextMeshProUGUI calculateText;
    public Button dealBtn;
    public TextMeshProUGUI curGoldText;
    
    private List<ShopSlotUI> sellSlots = new();
    private List<ShopSlotUI> buySlots = new ();
    
    private HashSet<ItemSlot> purchaseSlots = new(); //거래된 슬롯 기억리스트
    
    
    public override void Open()
    {
        base.Open();
        if (playerInventoryRaw == null)
        {
            var inventoryUI = UIManager.Instance.GetUI<InventoryUI>();
            if (inventoryUI != null)
                playerInventoryRaw = inventoryUI.GetComponent<Inventory>();
        }

        if (shopInventoryRaw == null)
            shopInventoryRaw = FindObjectOfType<ShopInventory>();
        
        UpdateGoldUI();
            
        dealBtn.onClick.RemoveAllListeners();
        dealBtn.onClick.AddListener(ExecuteTransaction);
        
        StartCoroutine(InitAndGenerate());
    }
    private IEnumerator InitAndGenerate()
    {
        yield return new WaitUntil(() =>
            playerInventoryRaw != null && playerInventoryRaw.Initialized &&
            shopInventoryRaw != null && shopInventoryRaw.Initialized);

        GenerateSlots();
        UpdateTotalPrices();
    }

    public void GenerateSlots()
    {
        ClearChildren(sellParent);
        ClearChildren(buyParent);
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
                
                slotUI.Set(itemSlot, isPlayer,this);
                targetList.Add(slotUI);

                if(!isPlayer && purchaseSlots.Contains(itemSlot))
                {
                    slotUI.SetInteractable(false);
                }
            }
        }
    }

    public void UpdateTotalPrices()
    {
        int sellTotal = 0;
        int buyTotal = 0;

        foreach (var s in sellSlots)
            if(s.IsSelected && s.ItemSlot != null && !s.ItemSlot.IsEmpty)
            {
                sellTotal += s.ItemSlot.Item.sellPrice;
            }

        foreach (var b in buySlots)
            if(b.IsSelected  && b.ItemSlot != null && !b.ItemSlot.IsEmpty)
            {
                buyTotal += b.ItemSlot.Item.buyPrice;
            }

        sellTotalText.text = $"판매 금액: {sellTotal} G";
        buyTotalText.text = $"구매 금액: {buyTotal} G";
        
        float calculatePrice = sellTotal - buyTotal;
        string sign = calculatePrice >= 0 ? "+" : "-";
        string calculatePriceText = $"{sign}{Mathf.Abs(calculatePrice)}";
        
        calculateText.text = $"총 계산된 금액: {calculatePriceText} G";
    }

    public void ExecuteTransaction()
    {
        var sellSlotSelected = sellSlots.FindAll(s => s.IsSelected);
        var buySlotSelected = buySlots.FindAll(s => s.IsSelected);
        
        var sellItems = sellSlotSelected.ConvertAll(s => s.ItemSlot);
        var buyItems = buySlotSelected.ConvertAll(s => s.ItemSlot);
        
        if (ShopManager.Instance.TryExecuteTransaction(sellItems, buyItems, out var result))
        {
            Debug.Log(result);
            foreach(var slot in buySlotSelected)
            {
                purchaseSlots.Add(slot.ItemSlot);
            }
            GenerateSlots();
            UpdateTotalPrices();
            UpdateGoldUI();
        }
        else
        {
            Debug.LogWarning(result);
        }

        UpdateTotalPrices();
    }

    private void ClearChildren(Transform t)
    {
        foreach (Transform child in t)
            Destroy(child.gameObject);
    }

    private void UpdateGoldUI()
    {
        if(ShopManager.Instance.TryGetGold(out float gold))
        {
            curGoldText.text = $"보유골드: {gold}G";
        }
        else
        {
            curGoldText.text = "보유골드: ???";
        }
    }
    
}
