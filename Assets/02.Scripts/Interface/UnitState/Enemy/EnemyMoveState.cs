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
        moveSpeedModifier = 1;
        base.StateEnter();
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        Move();

        if(!IsInChaseRange())
        {
            // IdleState로 변환
            Debug.Log("EnemyMoveState: Chase range exceeded, switching to IdleState.");
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    private void Move()
    {
        Vector3 movementDirection = GetMovementDirection();

        Rotate(movementDirection);

        Move(movementDirection);
    }

    private Vector3 GetMovementDirection()
    {
        Vector3 dir = (stateMachine.Target.transform.position - stateMachine.Enemy.transform.position);

        // 2D인 경우 x축 y축만 고려
        if(viewMode == ViewModeType.View2D)
        {
            dir.z = 0;
        }

        dir.Normalize();
        return dir;
    }

    void Move(Vector3 movementDirection)
    {
        float movementSpeed = GetMovementSpeed();

        // 2D인 경우 Rigidbody를 사용하고, 3D인 경우 NavMeshAgent를 사용
        if(viewMode == ViewModeType.View2D)
        {
            stateMachine.Enemy.Rigidbody.velocity = movementDirection * movementSpeed;
        }
        else
        {
            stateMachine.Enemy.NavMeshAgent.SetDestination(stateMachine.Target.transform.position);
        }
    }

    private float GetMovementSpeed()
    {
        float movementSpeed = stateMachine.MovementSpeed * moveSpeedModifier;
        return movementSpeed;
    }

    void Rotate(Vector3 movementDirection)
    {
        if(movementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            stateMachine.Enemy.transform.rotation = Quaternion.Lerp(stateMachine.Enemy.transform.rotation, targetRotation, stateMachine.Enemy.RotationDamping * Time.deltaTime);
        }
    }
}
