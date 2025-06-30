using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class EnemyMoveState : EnemyBaseState
{
    // StatePhysicsUpdate 메서드를 오버라이드하여 Base 이전에 초기화
    protected Vector3 targetPos;

    public EnemyMoveState(EnemyStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();

        stateMachine.Enemy.NavMeshAgent.isStopped = false; // NavMeshAgent를 정지시킴
    }

    public override void StateExit()
    {
        base.StateExit();
        stateMachine.Enemy.NavMeshAgent.isStopped = true; // NavMeshAgent를 정지시킴
        stateMachine.Enemy._Rigidbody.velocity = Vector3.zero; // Rigidbody를 정지시킴
    }

    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
        Move();
        Debug.Log(targetPos);
    }

    protected void Move()
    {
        Vector3 movementDirection = GetMovementDirection();

        Rotate(movementDirection);

        Move(movementDirection);
    }

    protected Vector3 GetMovementDirection()
    {
        Vector3 dir = targetPos - stateMachine.Enemy.transform.position;

        // 2D인 경우 x축 y축만 고려
        if(viewMode == ViewModeType.View2D)
        {
            dir.z = 0;
        }
        dir.y = 0; // y축은 현재 위치 유지

        dir.Normalize();
        Debug.Log($"GetMovementDirection: {dir}");
        return dir;
    }

    protected void Move(Vector3 movementDirection)
    {
        float movementSpeed = GetMovementSpeed();

        // 2D인 경우 Rigidbody를 사용하고, 3D인 경우 NavMeshAgent를 사용
        if(viewMode == ViewModeType.View2D)
        {
            stateMachine.Enemy.NavMeshAgent.isStopped = true; // NavMeshAgent를 정지시킴
            stateMachine.Enemy._Rigidbody.velocity = movementDirection * movementSpeed;
        }
        else
        {
            stateMachine.Enemy._Rigidbody.velocity = Vector3.zero; // Rigidbody를 정지시킴
            stateMachine.Enemy.NavMeshAgent.SetDestination(targetPos);
        }
    }

    protected float GetMovementSpeed()
    {
        float movementSpeed = stateMachine.MovementSpeed * moveSpeedModifier;
        return movementSpeed;
    }

    protected void Rotate(Vector3 movementDirection)
    {
        if(movementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            stateMachine.Enemy.transform.rotation = Quaternion.Lerp(stateMachine.Enemy.transform.rotation, targetRotation, stateMachine.Enemy.RotationDamping * Time.deltaTime);
        }
    }

    /// <summary>
    /// 현재 타겟까지의 거리를 반환하는 메서드
    /// </summary>
    /// <returns></returns>
    protected float GetDistanceToTarget()
    {
        if(viewMode == ViewModeType.View2D)
        {
            // 2D인 경우 x축과 y축만 고려하여 거리 계산
            Vector3 targetPos2D = new Vector3(targetPos.x, targetPos.y, stateMachine.Enemy.transform.position.z);
            return Vector3.Distance(stateMachine.Enemy.transform.position, targetPos2D);
        }

        return Vector3.Distance(stateMachine.Enemy.transform.position, targetPos);
    }
}
