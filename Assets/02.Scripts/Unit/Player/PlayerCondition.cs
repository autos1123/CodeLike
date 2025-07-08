using System;
using UnityEngine;

/// <summary>
/// 플레이어 스탯 및 Modifier 관리
/// BaseCondition 상속, FSM 연동, 피격 및 사망 처리 담당
/// </summary>
public class PlayerCondition:BaseCondition
{
    private PlayerStateMachine stateMachine;

    /// <summary>
    /// BaseCondition에 ConditionData 전달
    /// </summary>
    public PlayerCondition(ConditionData data) : base(data) { }
}
