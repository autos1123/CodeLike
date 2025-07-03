using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : EnemyMoveState
{
    public EnemyPatrolState(EnemyStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void StateEnter()
    {
        // Debug.LogWarning("PatrolState 진입");
        moveSpeedModifier = 1f;
        stateMachine.Enemy.NavMeshAgent.isStopped = false;
        base.StateEnter();

        targetPos = stateMachine.PatrolPoint;
        StartAnimation(stateMachine.Enemy.AnimationData.PatrolParameterHash);
    }

    public override void StateExit()
    {
        base.StateExit();

        StopAnimation(stateMachine.Enemy.AnimationData.PatrolParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if(IsInRange(ConditionType.ChaseRange))
        {
            // ChaseState로 전환
            stateMachine.ChangeState(stateMachine.ChaseState);
            return;
        }

        // 목표 지점에 도착한 경우
        if(IsArrivePatrolPoint())
        {
            // IdleState로 전환
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
    }

    public override void StatePhysicsUpdate()
    {
        targetPos = stateMachine.PatrolPoint;
        base.StatePhysicsUpdate();
    }

    private bool IsArrivePatrolPoint()
    {
        // 목표 지점까지의 거리가 0.2 이하인 경우 도착한 것으로 간주
        if(GetDistanceToTarget() <= 0.2f)
        {
            return true;
        }

        return false;
    }
}
