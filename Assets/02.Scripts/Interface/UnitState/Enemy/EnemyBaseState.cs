using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class EnemyBaseState:IUnitState
{
    protected EnemyStateMachine stateMachine;
    protected ConditionData data;

    protected ViewModeType viewMode;

    protected float moveSpeedModifier = 1f;

    public EnemyBaseState(EnemyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
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
