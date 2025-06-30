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
        base.StateEnter();
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if(!IsInChaseRange())
        {
            // IdleState로 변환
            stateMachine.Enemy.SetPatrolPivot();
            stateMachine.ChangeState(stateMachine.IdleState);
        }
        if(stateMachine.Enemy.GetTargetColliders(LayerMask.GetMask("Player")).Length > 0)
        {
            // AttackState로 변환
            stateMachine.ChangeState(stateMachine.AttackState);
        }
    }

    public override void StatePhysicsUpdate()
    {
        targetPos = stateMachine.Player.transform.position;
        base.StatePhysicsUpdate();
    }
}
