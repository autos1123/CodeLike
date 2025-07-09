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

    private IInventory playerInventory => GameManager.Instance.Player?.GetComponent<Inventory>();
    [SerializeField] private ShopInventory shopInventoryRaw;
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
    
    private HashSet<InventoryItemSlot> selectedSellItems = new(); //선택된 슬롯 기억리스트 (슬롯과 그 안에 아이템 함께기억)
    private HashSet<InventoryItemSlot> purchaseSlots = new(); //거래된 슬롯 기억리스트
    
    private int inventoryInitCount = 0;
    /// <summary>
    /// 상점 UI 열기: 인벤토리 참조 설정 및 거래 버튼 이벤트 등록, 슬롯 초기화 코루틴 시작
    /// </summary>
    public override void Open()
    {
        base.Open();

        if (shopInventoryRaw == null)
            shopInventoryRaw = FindObjectOfType<ShopInventory>();

        UpdateGoldUI();
        dealBtn.onClick.RemoveAllListeners();
        dealBtn.onClick.AddListener(ExecuteTransaction);

        if (shopInventoryRaw.Initialized && playerInventory.Initialized)
            InitAndGenerate();
        else
            WaitForInit();
    }
    
    /// <summary>
    /// 상점 UI 닫기 시 선택된 아이템 상태 초기화
    /// </summary>
    public override void Close()
    {
        base.Close();
        selectedSellItems.Clear();
    }
    
    // /// <summary>
    // /// 인벤토리 초기화 대기 후 슬롯 생성 및 UI 업데이트 수행 (코루틴)
    // /// </summary>
    // private IEnumerator InitAndGenerate()
    // {
    //     yield return new WaitUntil(() =>
    //         playerInventory  != null && playerInventory .Initialized &&
    //         shopInventoryRaw != null && shopInventoryRaw.Initialized);
    //
    //     GenerateSlots();
    //     UpdateTotalPrices();
    // }
    private void WaitForInit()
    {
        shopInventoryRaw.OnInitialized += OnInventoryReady;
        playerInventory.OnInitialized += OnInventoryReady;
    }

    private void OnInventoryReady()
    {
        inventoryInitCount++;
        if (inventoryInitCount >= 2)
        {
            shopInventoryRaw.OnInitialized -= OnInventoryReady;
            playerInventory.OnInitialized -= OnInventoryReady;
            InitAndGenerate();
        }
    }

    private void InitAndGenerate()
    {
        GenerateSlots();
        UpdateTotalPrices();
    }
    /// <summary>
    /// 기존 슬롯 UI 제거 후, 판매/구매 슬롯을 다시 생성
    /// </summary>
    /// 
    public void GenerateSlots()
    {
        ClearChildren(sellParent);
        ClearChildren(buyParent);
        sellSlots.Clear();
        buySlots.Clear();

        CreateSlots(playerInventory.GetInventorySlots(true), sellParent, true, sellSlots);
        CreateSlots(shopInventory.GetInventorySlots(), buyParent, false, buySlots);
    }
    
    /// <summary>
    /// 특정 슬롯 리스트로 상점 슬롯 UI 생성 및 리스트에 저장, 이전 선택 상태 반영
    /// </summary>
    /// <param name="slots">아이템 슬롯들</param>
    /// <param name="parent">UI 부모 오브젝트</param>
    /// <param name="isPlayer">플레이어 인벤토리인지 여부</param>
    /// <param name="targetList">슬롯 저장 리스트</param>
    private void CreateSlots(List<InventoryItemSlot> slots, Transform parent, bool isPlayer, List<ShopSlotUI> targetList)
    {
        foreach (var itemSlot in slots)
        {
            if (!itemSlot.IsEmpty)
            {
                var go = Instantiate(shopSlotPrefab, parent);
                var slotUI = go.GetComponent<ShopSlotUI>();
                
                slotUI.Set(itemSlot, isPlayer,this);
                targetList.Add(slotUI);
                slotUI.OnClicked += HandleSlotClick;
                
                if(!isPlayer && purchaseSlots.Contains(itemSlot))
                {
                    slotUI.SetInteractable(false);
                }
                if (isPlayer && selectedSellItems.Contains(itemSlot))
                {
                    slotUI.ForceSelect();
                }
            }
        }
    }
    private void HandleSlotClick(ShopSlotUI slotUI)
    {
        var slot = slotUI.InventoryItemSlot;

        // 플레이어 아이템이고 장착 상태라면
        if (slotUI.isPlayerSlot && IsEquippedSlot(slot))
        {
            UIManager.Instance.ShowConfirmPopup(
                "장착한 아이템입니다. 장착 해제하시겠습니까?",
                onConfirm: () =>
                {
                    RememberSelectedItem(slot); // 거래 대상으로 등록
                    EquipmentManager.Instance.UnEquip(slot); // 장착 해제
                    RefreshAllUI(); // 전체 UI 다시 그림
                },
                onCancel: () =>
                {
                    Debug.Log("선택 취소됨");
                });
        }
        else
        {
            // 선택 상태만 변경
            slotUI.ToggleSelect();
            UpdateTotalPrices();
        }
    }
    /// <summary>
    /// 선택된 슬롯들의 판매/구매 금액 계산 후 텍스트 갱신
    /// </summary>
    public void UpdateTotalPrices()
    {
        int sellTotal = 0;
        int buyTotal = 0;

        foreach (var s in sellSlots)
            if(s.IsSelected && s.InventoryItemSlot != null && !s.InventoryItemSlot.IsEmpty)
            {
                sellTotal += s.InventoryItemSlot.InventoryItem.sellPrice;
            }

        foreach (var b in buySlots)
            if(b.IsSelected  && b.InventoryItemSlot != null && !b.InventoryItemSlot.IsEmpty)
            {
                buyTotal += b.InventoryItemSlot.InventoryItem.buyPrice;
            }

        sellTotalText.text = $"판매 금액: {sellTotal} G";
        buyTotalText.text = $"구매 금액: {buyTotal} G";
        
        float calculatePrice = sellTotal - buyTotal;
        string sign = calculatePrice >= 0 ? "+" : "-";
        string calculatePriceText = $"{sign}{Mathf.Abs(calculatePrice)}";
        
        calculateText.text = $"총 계산된 금액: {calculatePriceText} G";
    }
    
    /// <summary>
    /// 선택된 슬롯을 기반으로 거래 시도 → 성공 시 거래 아이템 반영 및 UI 갱신
    /// </summary>
    public void ExecuteTransaction()
    {
        var sellSlotSelected = sellSlots.FindAll(s => s.IsSelected);
        var buySlotSelected = buySlots.FindAll(s => s.IsSelected);
        
        var sellItems = sellSlotSelected.ConvertAll(s => s.InventoryItemSlot);
        var buyItems = buySlotSelected.ConvertAll(s => s.InventoryItemSlot);
        
        if (ShopManager.Instance.TryExecuteTransaction(sellItems, buyItems, out var result))
        {
            Debug.Log(result);
            foreach(var slot in buySlotSelected)
            {
                purchaseSlots.Add(slot.InventoryItemSlot);
            }
            selectedSellItems.Clear();
            RefreshAllUI();
        }
        else
        {
            Debug.LogWarning(result);
        }

        UpdateTotalPrices();
    }
    
    /// <summary>
    /// 부모 Transform의 모든 자식 오브젝트 제거 (슬롯 재생성 전 사용)
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
    /// UI갱신메소드 모음(슬롯, 가격, 골드 UI 전부 다시 갱신)
    /// </summary>
    public void RefreshAllUI()
    {
        GenerateSlots();
        UpdateTotalPrices();
        UpdateGoldUI();
    }
    /// <summary>
    /// 주어진 슬롯이 장착 슬롯에 포함되어 있는지 여부 확인
    /// </summary>
    /// <param name="slot">선택한 슬롯</param>
    /// <returns>그 슬롯이 equipslots인지 불값 반환</returns>
    public bool IsEquippedSlot(InventoryItemSlot slot)
    {
        var inventory = GameManager.Instance.Player?.GetComponent<Inventory>();
        return inventory != null && inventory.equipSlots.Contains(slot);
    }
    /// <summary>
    /// 특정 슬롯+아이템 쌍을 선택된 리스트에 등록
    /// </summary>
    public void RememberSelectedItem(InventoryItemSlot slot)
    {
        if (slot != null)
            selectedSellItems.Add(slot);
    }
    /// <summary>
    /// 특정 슬롯+아이템 쌍을 선택된 리스트에서 제거
    /// </summary>
    public void ForgetSelectedItem(InventoryItemSlot slot)
    {
        if (slot != null)
            selectedSellItems.Remove(slot);
    }
}
