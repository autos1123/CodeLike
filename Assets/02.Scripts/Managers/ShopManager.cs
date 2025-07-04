using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상점 매니저: 거래 처리 및 플레이어 상태 정보 관리
/// </summary>
public class ShopManager:MonoSingleton<ShopManager>
{
    private ConditionData playerCondition;
    private IInventory playerInventory;
    
    /// <summary>
    /// 테이블 로딩 여부 확인 후, 참조 초기화 메서드 등록 또는 실행
    /// </summary>
    private void Start()
    {
        if (!TableManager.Instance.loadComplete)
            TableManager.Instance.loadComplet += SetupReferences;
        else
            SetupReferences();
    }

    /// <summary>
    /// 플레이어의 ConditionData 및 인벤토리 참조 설정
    /// </summary>
    private void SetupReferences()
    {
        var conditionTable = TableManager.Instance.GetTable<ConditionDataTable>();
        if (conditionTable == null)
        {
            Debug.LogError("ConditionDataTable을 찾을 수 없습니다.");
            return;
        }

        playerCondition = conditionTable.GetDataByID(0); // 플레이어 ID = 0
        if (playerCondition == null)
        {
            Debug.LogError("ID 0인 ConditionData를 찾을 수 없습니다.");
            return;
        }

        playerCondition.InitConditionDictionary();
        
        // 인벤토리 로딩은 비동기 UI 로딩 완료까지 대기
        StartCoroutine(SetupInventoryWhenReady());
    }
    
    /// <summary>
    /// UIManager에서 InventoryUI가 준비될 때까지 대기 후 인벤토리 참조 설정
    /// </summary>
    private IEnumerator SetupInventoryWhenReady()
    {
        Dictionary<string, UIBase> dic = typeof(UIManager)
            .GetField("_uiInstances", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(UIManager.Instance) as Dictionary<string, UIBase>;

        if (dic == null)
        {
            Debug.LogError("UIManager 내부 딕셔너리를 찾을 수 없습니다.");
            yield break;
        }

        yield return new WaitUntil(() => dic.ContainsKey(nameof(InventoryUI)));

        var inventoryUI = dic[nameof(InventoryUI)] as InventoryUI;
        playerInventory = inventoryUI.GetComponent<Inventory>();
    }
    
    /// <summary>
    /// 거래 실행: 판매/구매 아이템 확인 및 골드 계산, 인벤토리 반영
    /// </summary>
    public bool TryExecuteTransaction(List<ItemSlot> sellItems, List<ItemSlot> buyItems, out string result)
    {
        int sellTotal = CalculateTotalPrice(sellItems, true, out string sellError);
        if (sellTotal < 0)
        {
            result = sellError;
            return false;
        }

        int buyTotal = CalculateTotalPrice(buyItems, false, out string buyError);
        if (buyTotal < 0)
        {
            result = buyError;
            return false;
        }

        if(!playerCondition.TryGetCondition(ConditionType.Gold, out float currentGold))
        {
            result = "골드 정보 없음";
            return false;
        }

        float newGold = currentGold + sellTotal - buyTotal;
        if(newGold < 0)
        {
            result = "골드 부족";
            return false;
        }

        // 골드 반영
        playerCondition.Conditions[ConditionType.Gold].SetValue(newGold);

        // 아이템 제거/추가 (기존 메서드 사용)
        foreach(var slot in sellItems)
            playerInventory.RemoveFromInventory(slot.Item);

        foreach(var slot in buyItems)
            playerInventory.AddToInventory(slot.Item);

        result = $"거래 완료! 현재 골드: {newGold}";
        return true;
    }

    /// <summary>
    /// 플레이어의 현재 골드를 가져옴
    /// </summary>
    public bool TryGetGold(out float gold)
    {
        if(playerCondition != null && playerCondition.TryGetCondition(ConditionType.Gold, out float currentGold))
        {
            gold = currentGold;
            return true;
        }
        gold = 0;
        return false;
    }
    
    /// <summary>
    /// 아이템 슬롯 리스트의 총 가격 계산 및 유효성 확인
    /// </summary>
    private int CalculateTotalPrice(List<ItemSlot> slots, bool isSell, out string error)
    {
        int total = 0;
        error = string.Empty;

        foreach (var slot in slots)
        {
            if (slot.Quantity != 1)
            {
                error = "수량이 1개가 아닌 경우는 지원하지 않습니다.";
                return -1;
            }
            total += isSell ? slot.Item.sellPrice : slot.Item.buyPrice;
        }

        return total;
    }
}
