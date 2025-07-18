using System;
using UnityEngine;

public static class GameEvents
{
    // 몬스터 처치 시 호출
    public static event Action OnMonsterKilled;
    // 아이템 착용 시 호출 (착용된 아이템의 고유 ID를 전달)
    public static event Action<string> OnItemEquipped;
    // 아이템 획득 시 호출 (획득된 아이템의 고유 ID를 전달)
    public static event Action<string> OnItemAcquired;
    
    public static void TriggerMonsterKilled()
    {
        OnMonsterKilled?.Invoke();
        Debug.Log("GameEvents: OnMonsterKilled 이벤트가 발생했습니다");
    }
}
