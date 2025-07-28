using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDownState : EnemyBaseState
{
    private float recoveryTerm = 10f; // 부활 대기 시간
    private float recoveryTimer = 0f; // 부활 타이머

    public BossDownState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void StateEnter()
    {
        moveSpeedModifier = 0; // Idle 상태에서는 이동 속도를 0으로 설정
        stateMachine.Enemy.NavMeshAgent.enabled = false; // NavMeshAgent를 정지시킴
        stateMachine.Enemy._Rigidbody.isKinematic = true; // Rigidbody를 정지시킴
        stateMachine.Enemy.col.enabled = false; // 충돌체 비활성화 (부활 중에는 충돌하지 않도록)
        base.StateEnter();

        recoveryTimer = 0; // 부활 타이머 초기화
        stateMachine.Enemy._Animator.SetTrigger(stateMachine.Enemy.AnimationData.DieParameterHash);
    }

    public override void StateExit()
    {
        stateMachine.Enemy.NavMeshAgent.enabled = true; // NavMeshAgent를 정지시킴
        stateMachine.Enemy._Rigidbody.isKinematic = false; // Rigidbody를 정지시킴
        stateMachine.Enemy.col.enabled = true; // 충돌체 비활성화 (부활 중에는 충돌하지 않도록)

        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        // 회복 시간 대기 후 부활
        recoveryTimer += Time.deltaTime;
        stateMachine.Enemy.Condition.Heal(Time.deltaTime / recoveryTerm * stateMachine.Enemy.Condition.GetTotalMaxValue(ConditionType.HP)); // HP 회복

        if(recoveryTimer >= recoveryTerm)
        {
            stateMachine.Enemy.Condition.Heal(stateMachine.Enemy.Condition.GetTotalMaxValue(ConditionType.HP));
            // 부활 후 Idle 상태로 전환
            if(stateMachine.ChangeState(EnemyStateType.Idle))
                return;
        }
    }
}
