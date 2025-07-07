using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 상점 UI. 판매/구매 슬롯 생성, 거래 처리, 금액 계산 및 UI 갱신을 담당
/// </summary>
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
    
    /// <summary>
    /// 상점 UI 열기: 초기 인벤토리 참조, 버튼 이벤트 등록, 슬롯 생성 시작
    /// </summary>
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
    
    /// <summary>
    /// 인벤토리들이 완전히 초기화될 때까지 대기한 후 슬롯 생성 및 가격 갱신
    /// </summary>
    private IEnumerator InitAndGenerate()
    {
        yield return new WaitUntil(() =>
            playerInventoryRaw != null && playerInventoryRaw.Initialized &&
            shopInventoryRaw != null && shopInventoryRaw.Initialized);

        GenerateSlots();
        UpdateTotalPrices();
    }
    /// <summary>
    /// 판매/구매 슬롯을 전부 삭제 후 새로 생성 (UI 갱신)
    /// </summary>
    public void GenerateSlots()
    {
        ClearChildren(sellParent);
        ClearChildren(buyParent);
        sellSlots.Clear();
        buySlots.Clear();

        CreateSlots(playerInventory.GetInventorySlots(), sellParent, true, sellSlots);
        CreateSlots(shopInventory.GetInventorySlots(), buyParent, false, buySlots);
    }
    
    /// <summary>
    /// 주어진 ItemSlot 리스트로 UI 슬롯을 생성하여 슬롯 리스트에 추가
    /// </summary>
    /// <param name="slots">아이템 슬롯들</param>
    /// <param name="parent">UI 부모 오브젝트</param>
    /// <param name="isPlayer">플레이어 인벤토리인지 여부</param>
    /// <param name="targetList">슬롯 저장 리스트</param>
    private void CreateSlots(List<ItemSlot> slots, Transform parent, bool isPlayer, List<ShopSlotUI> targetList)
    {
        foreach (var itemSlot in slots)
        {
            if (!itemSlot.IsInvenSlotEmpty)
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
    
    /// <summary>
    /// 현재 선택된 슬롯들을 기준으로 판매/구매 금액 계산 및 표시
    /// </summary>
    public void UpdateTotalPrices()
    {
        int sellTotal = 0;
        int buyTotal = 0;

        foreach (var s in sellSlots)
            if(s.IsSelected && s.ItemSlot != null && !s.ItemSlot.IsInvenSlotEmpty)
            {
                sellTotal += s.ItemSlot.Item.sellPrice;
            }

        foreach (var b in buySlots)
            if(b.IsSelected  && b.ItemSlot != null && !b.ItemSlot.IsInvenSlotEmpty)
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
    
    /// <summary>
    /// 거래 실행: 선택된 아이템을 ShopManager에 전달하고 결과 처리.  
    /// 거래 성공 시 구매 아이템 비활성화 후 UI 갱신
    /// </summary>
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

            RefreshAllUI();
        }
        else
        {
            Debug.LogWarning(result);
        }

        UpdateTotalPrices();
    }
    
    /// <summary>
    /// 특정 Transform의 모든 자식 오브젝트 제거
    /// </summary>
    private void ClearChildren(Transform t)
    {
        foreach (Transform child in t)
            Destroy(child.gameObject);
    }
    
    /// <summary>
    /// 현재 플레이어 골드를 UI에 표시
    /// </summary>
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
    /// <summary>
    /// UI갱신메소드 모음
    /// </summary>
    private void RefreshAllUI()
    {
        GenerateSlots();
        UpdateTotalPrices();
        UpdateGoldUI();
    }
    
}
