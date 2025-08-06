using System.Collections.Generic;
using UnityEngine;

public enum EnemyStateType
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Die,
    Hit,
    Dash
}

public class EnemyStateMachine : UnitStateMachine
{
    public EnemyController Enemy { get; }

    public float MovementSpeed { get; private set; }

    public GameObject Player { get; private set; }
    public Vector3 PatrolPoint { get; private set; }

    public Dictionary<EnemyStateType, EnemyBaseState> States { get; } = new Dictionary<EnemyStateType, EnemyBaseState>();

    public EnemyStateType CurrentStateType { get; private set; }

    public EnemyStateMachine(EnemyController enemy)
    {
        this.Enemy = enemy;
        Player = GameManager.Instance.Player;
        MovementSpeed = Enemy.Condition.GetTotalCurrentValue(ConditionType.MoveSpeed);
    }

    public void AddState(EnemyStateType stateType, EnemyBaseState state)
    {
        States[stateType] = state;
    }


    public void StartStateMachine(EnemyStateType stateType)
    {
        Enemy.SetPatrolPivot();
        ChangeState(States[stateType]);
    }

    public bool HasState(EnemyStateType stateType)
    {
        return States.ContainsKey(stateType);
    }

    public bool ChangeState(EnemyStateType stateType)
    {
        if(States.TryGetValue(stateType, out EnemyBaseState state))
        {
            CurrentStateType = stateType;
            ChangeState(state);
            return true;
        }
        else
        {
            Debug.LogWarning($"State {stateType} not found in EnemyStateMachine.");
            return false;
        }
    }

    /// <summary>
    /// 타겟의 Transform 대입
    /// 실시간 위치 추적이 필요한 경우
    /// </summary>
    /// <param name="target"></param>
    public void SetPatrolPoint(Vector3 target)
    {
        PatrolPoint = target;
    }
}
