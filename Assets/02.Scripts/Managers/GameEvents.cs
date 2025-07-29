using System;
using UnityEngine;

public static class GameEvents
{
    // 몬스터 처치 시 호출
    public static event Action OnMonsterKilled;
    // TakePassiveItem UI가 열렸을 때 호출
    public static event Action OnPassiveItemUIOpened;
    // 패시브아이템 획득 시 호출
    public static event Action OnPassiveItemTake;
    // TakeActiveItem UI가 열렸을 때 호출
    public static event Action OnActiveItemUIOpened;
    // 액티브아이템 획득 시 호출
    public static event Action OnActiveItemTake;
    //액티브스킬쓸때 호출
    public static event Action OnActiveSkillUse;
    //인베토리 열때 호출
    public static event Action OnInventoryOpened;
    // 아이템 착용 시 호출
    public static event Action OnItemEquipped;

    public static void TriggerMonsterKilled()
    {
        OnMonsterKilled?.Invoke();
        Debug.Log("GameEvents: OnMonsterKilled 이벤트가 발생했습니다");
    }
    public static void TriggerPassiveItemUIOpened()
    {
        OnPassiveItemUIOpened?.Invoke();
        Debug.Log("GameEvents: OnRandomItemUIOpened 이벤트가 발생했습니다.");
    }
    public static void TriggerPassiveItemTake()
    {
        OnPassiveItemTake?.Invoke();
        Debug.Log("GameEvents: OnItemAcquired 이벤트가 발생했습니다.");
    }
    public static void TriggerActiveItemUIOpened()
    {
        OnActiveItemUIOpened?.Invoke();
        Debug.Log("GameEvents: OnRandomItemUIOpened 이벤트가 발생했습니다.");
    }
    public static void TriggerActiveItemTake()
    {
        OnActiveItemTake?.Invoke();
        Debug.Log("GameEvents: OnActiveItemTake 이벤트가 발생했습니다.");
    }

    public static void TriggerActiveSkillUse()
    {
        OnActiveSkillUse?.Invoke();
        Debug.Log("GameEvents: OnActiveSkillUse 이벤트가 발생했습니다.");
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
