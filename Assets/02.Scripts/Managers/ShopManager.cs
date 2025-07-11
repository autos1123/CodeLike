using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상점 매니저: 거래 처리 및 플레이어 상태 정보 관리
/// </summary>
public class ShopManager:MonoSingleton<ShopManager>
{
    
    private IInventory playerInventory;
    
    private void Start()
    {
        StartCoroutine(InitializeShopManager());
    }

    
    private IEnumerator InitializeShopManager()
    {
        yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.Player != null);
        SetupInventory();
    }
    
    /// <summary>
    /// 플레이어 오브젝트에서 직접 인벤토리 참조
    /// </summary>
    private void SetupInventory()
    {
        var player = GameManager.Instance?.Player;

        if (player == null)
        {
            Debug.LogError("GameManager.Instance.Player가 null입니다.");
            return;
        }

        playerInventory = player.GetComponent<Inventory>();

        if (playerInventory == null)
        {
            Debug.LogError("Player 오브젝트에서 Inventory 컴포넌트를 찾을 수 없습니다.");
        }
    }
    
    /// <summary>
    /// 선택된 아이템 기반으로 골드 계산 후 거래 가능 여부 판단 및 실제 아이템 처리
    /// </summary>
    public bool TryExecuteTransaction(List<InventoryItemSlot> sellItems, List<InventoryItemSlot> buyItems, out string result)
    {
        PlayerController playerController = GameManager.Instance.Player.GetComponent<PlayerController>();
        BaseCondition playerCondition = playerController?.Condition;
        
        if (playerCondition == null)
        {
            result = "거래 실패: 플레이어 상태 정보가 초기화되지 않았습니다.";
            Debug.LogError(result);
            return false;
        }
        
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
        
        if(!playerCondition.CurrentConditions.TryGetValue(ConditionType.Gold, out float currentGold))
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
        playerCondition.ChangeGold(newGold - currentGold);

        // 아이템 제거/추가 (기존 메서드 사용)
        foreach(var slot in sellItems)
            playerInventory.RemoveFromInventory(slot.InventoryItem);

        foreach(var slot in buyItems)
            playerInventory.AddToInventory(slot.InventoryItem);

        result = $"거래 완료! 현재 골드: {newGold}";
        return true;
    }
    
    /// <summary>
    /// 아이템 슬롯 리스트의 총 가격 계산 및 유효성 확인
    /// </summary>
    private int CalculateTotalPrice(List<InventoryItemSlot> slots, bool isSell, out string error)
    {
        int total = 0;
        error = string.Empty;

        foreach (var slot in slots)
        {
            total += isSell ? slot.InventoryItem.sellPrice : slot.InventoryItem.buyPrice;
        }

        return total;
    }
}
