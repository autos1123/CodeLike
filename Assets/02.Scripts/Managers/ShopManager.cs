using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager:MonoSingleton<ShopManager>
{
    private ConditionData playerCondition;
    private IInventory playerInventory;

    public ShopManager(ConditionData condition)
    {
        playerCondition = condition;
    }
    private void Start()
    {
        if (!TableManager.Instance.loadComplete)
            TableManager.Instance.loadComplet += SetupReferences;
        else
            SetupReferences();
    }

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
    public bool TryExecuteTransaction(List<ItemSlot> sellItems, List<ItemSlot> buyItems, out string result)
    {
        int sellTotal = 0;
        int buyTotal = 0;

        foreach(var slot in sellItems)
        {
            if(slot.Quantity != 1)
            {
                result = "수량이 1개가 아닌 경우는 지원하지 않습니다.";
                return false;
            }

            sellTotal += slot.Item.sellPrice;
        }

        foreach(var slot in buyItems)
        {
            if(slot.Quantity != 1)
            {
                result = "수량이 1개가 아닌 경우는 지원하지 않습니다.";
                return false;
            }

            buyTotal += slot.Item.buyPrice;
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
}
