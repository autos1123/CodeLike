using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : UnitStateMachine
{
    public EnemyController Enemy { get; }

    public float MovementSpeed { get; private set; }

    public GameObject Player { get; private set; }
    public Vector3 PatrolPoint { get; private set; }

    // State
    public EnemyIdleState IdleState { get; }
    public EnemyPatrolState PatrolState { get; }
    public EnemyChaseState ChaseState { get; }

    public EnemyStateMachine(EnemyController enemy)
    {
        this.Enemy = enemy;
        Player = GameObject.FindGameObjectWithTag("Player");

        if(Enemy.Data.TryGetCondition(ConditionType.MoveSpeed, out float value))
        {
            MovementSpeed = value;
        }
        else
        {
            Debug.LogError($"ConditionType MoveSpeed를 찾을 수 없습니다. 기본값으로 1.0f를 사용합니다.");
            MovementSpeed = 1.0f;
        }

        IdleState = new EnemyIdleState(this);
        PatrolState = new EnemyPatrolState(this);
        ChaseState = new EnemyChaseState(this);

        Enemy.SetPatrolPivot();
        ChangeState(IdleState);
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
