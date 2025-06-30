using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    float attackDelay;
    float startTime;

    public EnemyAttackState(EnemyStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void StateEnter()
    {
        moveSpeedModifier = 0f; // 공격 상태에서는 이동하지 않음
        if(!stateMachine.Enemy.Data.TryGetCondition(ConditionType.AttackSpeed, out float atkSpeed))
            atkSpeed = 1.0f; // 기본 공격 속도 설정

        attackDelay = 1.0f / atkSpeed; // 공격 속도에 따라 딜레이 설정
        startTime = Time.time;
        base.StateEnter();
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if(Time.time - startTime < attackDelay)
        {
            // 공격 딜레이가 끝나지 않았으면 대기
            return;
        }

        // 공격 행동 수행
        stateMachine.Enemy.AttackAction();
        Debug.Log($"Enemy {stateMachine.Enemy.name} attacks!");
        startTime = Time.time; // 다음 공격을 위한 시간 초기화
    }
}
