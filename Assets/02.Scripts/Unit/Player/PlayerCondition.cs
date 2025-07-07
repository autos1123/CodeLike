using System;
using UnityEngine;

/// <summary>
/// 플레이어 스탯 및 Modifier 관리
/// BaseCondition 상속, FSM 연동, 피격 및 사망 처리 담당
/// </summary>
public class PlayerCondition:BaseCondition
{
    private PlayerStateMachine stateMachine;
    private PlayerController controller;

    /// <summary>
    /// HP UI 갱신을 위한 현재 HP, 최대 HP 전달 이벤트
    /// </summary>
    public event Action<float, float> OnHPChanged;

    /// <summary>
    /// BaseCondition에 ConditionData 전달
    /// </summary>
    public PlayerCondition(ConditionData data) : base(data) { }

    /// <summary>
    /// FSM 및 컨트롤러 연결 초기화
    /// </summary>
    public void Init(PlayerController player, PlayerStateMachine stateMachine)
    {
        this.controller = player;
        this.stateMachine = stateMachine;
    }

    /// <summary>
    /// 데미지를 받아 HP 감소 및 UI 갱신, 사망 시 FSM Dead 상태 전환
    /// </summary>
    public bool TakenDamage(float damage)
    {
        bool isDead = base.GetDamaged(damage);

        float currentHP = GetValue(ConditionType.HP);
        float maxHP = 0f;
        if(!Data.TryGetCondition(ConditionType.HP, out maxHP))
        {
            Debug.LogError("[PlayerCondition] 최대 HP 정보가 없습니다.");
            maxHP = 1f;
        }

        OnHPChanged?.Invoke(currentHP, maxHP);

        // ✅ HUD의 ChangeHP()가 호출되도록 연결
        statModifiers[ConditionType.HP]?.Invoke();

        if(isDead)
        {
            Die();
            return true;
        }

        return false;
    }


    /// <summary>
    /// 사망 시 FSM Dead 상태로 전환
    /// </summary>
    private void Die()
    {
        stateMachine.ChangeState(new PlayerDeadState(controller, stateMachine));
    }
}
