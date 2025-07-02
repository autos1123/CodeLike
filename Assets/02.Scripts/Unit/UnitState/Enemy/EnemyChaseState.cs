using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyMoveState
{
    public EnemyChaseState(EnemyStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void StateEnter()
    {
        moveSpeedModifier = 1.5f;
        stateMachine.Enemy.NavMeshAgent.isStopped = false; 
        base.StateEnter();

        StartAnimation(stateMachine.Enemy.AnimationData.ChaseParameterHash);
    }

    public override void StateExit()
    {
        base.StateExit();

        StopAnimation(stateMachine.Enemy.AnimationData.ChaseParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if(!IsInRange(ConditionType.ChaseRange))
        {
            // IdleState로 변환
            stateMachine.Enemy.SetPatrolPivot();
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
        if(IsInRange(ConditionType.AttackRange))
        {
            // AttackState로 변환
            stateMachine.ChangeState(stateMachine.AttackState);
            return;
        }
    }

    public override void StatePhysicsUpdate()
    {
        targetPos = stateMachine.Player.transform.position;
        base.StatePhysicsUpdate();
    }
}
