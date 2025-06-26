using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class EnemyBaseState:IUnitState
{
    protected EnemyStateMachine stateMachine;
    protected ConditionData data;

    private ViewModeType viewMode;

    protected float moveSpeedModifier = 1f;

    public EnemyBaseState(EnemyStateMachine playerStateMachine)
    {
        stateMachine = playerStateMachine;
        data = stateMachine.Enemy.Data;
    }

    public virtual void StateEnter()
    {
        if(ViewManager.HasInstance)
        {
            viewMode = ViewManager.Instance.CurrentViewMode;
            ViewManager.Instance.OnViewChanged += SwitchView;
        }
        else
        {
            viewMode = ViewModeType.View2D; // 기본값 설정
        }
    }

    public virtual void StateExit()
    {
        if(ViewManager.HasInstance)
        {
            ViewManager.Instance.OnViewChanged -= SwitchView;
        }
    }

    public virtual void StatePhysicsUpdate()
    {
    }

    public virtual void StateUpdate()
    {
        Move();
    }

    /// <summary>
    /// 2D/3D 시점 변환시 호출되는 메서드
    /// </summary>
    /// <param name="mode"></param>
    public void SwitchView(ViewModeType mode)
    {
        if(viewMode == mode) return;
        viewMode = mode;
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
