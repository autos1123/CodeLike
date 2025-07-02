using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 플레이어 스탯 및 Modifier 관리
/// BaseCondition 상속, FSM 연동, 피격 및 사망 처리 담당
/// </summary>
public class PlayerCondition : BaseCondition
{
    
    private PlayerStateMachine stateMachine;
    private PlayerController controller;

    // (currentHP, maxHP) 전달
    public event Action<float, float> OnHPChanged;

    /// <summary>
    /// 생성자 - BaseCondition에 ConditionData 전달
    /// </summary>
    public PlayerCondition(ConditionData data) : base(data) { }

    /// <summary>
    /// FSM 및 컨트롤러 연결 초기화
    /// </summary>
    public void Init(PlayerController player, PlayerStateMachine stateMachine)
    {
        this.controller = player;
        this.stateMachine = stateMachine;
        Debug.Log($"초기 HP: {GetValue(ConditionType.HP)}");
    }

    /// <summary>
    /// BaseCondition의 GetDamaged 확장 (피격 및 사망 FSM 연동)
    /// 임시 메서드
    /// </summary>
    public bool TakenDamage(float damage)
    {
        Debug.Log($"{damage} 데미지 받음 처리 중");

        bool isDead = base.GetDamaged(damage);

        float currentHP = GetValue(ConditionType.HP);
        Debug.Log($"현재 HP: {currentHP}");
        Debug.Log($"isDead: {isDead}, HP after damage: {currentHP}");

        if(isDead)
        {
            Die();
            return true; // 사망
        }

        return false; // 생존
    }

    public float GetCurrentHpRatio()
    {
        if(!Data.TryGetCondition(ConditionType.HP, out float max))
        {
            Debug.LogError("ConditionType.HP를 찾을 수 없습니다.");
            return 0;
        }

        return GetValue(ConditionType.HP) / max;
    }


    /// <summary>
    /// 사망 시 FSM Dead 상태로 전환
    /// </summary>
    private void Die()
    {
        Debug.Log("PlayerCondition: 사망 -> PlayerDeadState 전환");
        stateMachine.ChangeState(new PlayerDeadState(controller, stateMachine));
    }
}
