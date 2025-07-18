using System;
using UnityEngine;

public static class GameEvents
{
    // 몬스터 처치 시 호출
    public static event Action OnMonsterKilled;
    // TakePassiveItem UI가 열렸을 때 호출
    public static event Action OnRandomItemUIOpened;
    // 아이템 획득 시 호출
    public static event Action OnItemTake;
    //인베토리 열때 호출
    public static event Action OnInventoryOpened;
    // 아이템 착용 시 호출
    public static event Action OnItemEquipped;
    public static void TriggerMonsterKilled()
    {
        OnMonsterKilled?.Invoke();
        Debug.Log("GameEvents: OnMonsterKilled 이벤트가 발생했습니다");
    }
    public static void TriggerRandomItemUIOpened()
    {
        OnRandomItemUIOpened?.Invoke();
        Debug.Log("GameEvents: OnRandomItemUIOpened 이벤트가 발생했습니다.");
    }
    public static void TriggerItemTake()
    {
        OnItemTake?.Invoke();
        Debug.Log("GameEvents: OnItemAcquired 이벤트가 발생했습니다.");
    }

    public static void TriggerInventoryOpened()
    {
        OnInventoryOpened?.Invoke();
        Debug.Log("GameEvents: OnInventoryOpened 이벤트가 발생했습니다.");
    }
    public static void TriggerItemEquipped()
    {
        OnItemEquipped?.Invoke();
        Debug.Log("GameEvents: OnItemEquipped 이벤트가 발생했습니다.");
    }
}
