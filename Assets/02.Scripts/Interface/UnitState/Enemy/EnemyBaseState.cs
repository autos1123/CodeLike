using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseState : IUnitState
{
    protected EnemyStateMachine stateMachine;
    protected ConditionData data;

    protected float moveSpeedModifier = 1f;

    public EnemyBaseState(EnemyStateMachine playerStateMachine)
    {
        stateMachine = playerStateMachine;
        data = stateMachine.Enemy.Condition.Data;
    }

    public virtual void StateEnter()
    {
    }

    public virtual void StateExit()
    {
    }

    public virtual void StatePhysicsUpdate()
    {
    }

    public virtual void StateUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 movementDirection = GetMovementDirection();

        Rotate(movementDirection);

        Move(movementDirection);
    }

    private Vector3 GetMovementDirection()
    {
        // 2D/3D 고려 필요
        Vector3 dir = (stateMachine.Target.transform.position - stateMachine.Enemy.transform.position).normalized;
        return dir;
    }

    void Move(Vector3 movementDirection)
    {
        float movementSpeed = GetMovementSpeed();
        // stateMachine.Enemy.Controller.Move(((movementDirection * movementSpeed) + stateMachine.Enemy.ForceReceiver.Movement) * Time.deltaTime);
    }

    private float GetMovementSpeed()
    {
        float movementSpeed = stateMachine.MovementSpeed * moveSpeedModifier;
        return movementSpeed;
    }

    void Rotate(Vector3 movementDirection)
    {
        // 2D/3D 고려 필요
        //if(movementDirection != Vector3.zero)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
        //    stateMachine.Enemy.transform.rotation = Quaternion.Lerp(stateMachine.Enemy.transform.rotation, targetRotation, stateMachine.RotationDamping * Time.deltaTime);
        //}
    }

    protected void ForceMove()
    {
        // stateMachine.Enemy.Controller.Move(stateMachine.Enemy.ForceReceiver.Movement * Time.deltaTime);
    }

    protected bool IsInChaseRange()
    {
        float playerDistanceSqr = (stateMachine.Target.transform.position - stateMachine.Enemy.transform.position).sqrMagnitude;
        float chaseRange = 0f;

        if(!data.TryGetCondition(ConditionType.ChaseRange, out chaseRange))
        {
            Debug.LogError($"ConditionType ChaseRange를 찾을 수 없습니다. 기본값으로 10.0f를 사용합니다.");
            chaseRange = 10.0f; // 기본값 설정
        }

        return playerDistanceSqr <= chaseRange * chaseRange;
    }
}
