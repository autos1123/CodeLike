using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class EnemyMoveState : EnemyBaseState
{

    public EnemyMoveState(EnemyStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void StateEnter()
    {
        stateMachine.Enemy.NavMeshAgent.isStopped = false; // NavMeshAgent를 정지시킴
        base.StateEnter();
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate(); 
        Vector3 movementDirection = GetMovementDirection(targetPos);

        Rotate(movementDirection);
    }

    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
        Vector3 movementDirection = GetMovementDirection(targetPos);

        Move(movementDirection);
    }
}
