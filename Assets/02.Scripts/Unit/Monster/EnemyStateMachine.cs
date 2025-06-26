using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : UnitStateMachine
{
    public EnemyController Enemy { get; }

    public float MovementSpeed { get; private set; }

    public GameObject Target { get; private set; }

    public EnemyStateMachine(EnemyController enemy)
    {
        this.Enemy = enemy;
        Target = GameObject.FindGameObjectWithTag("Player");

        if(Enemy.Condition.Data.TryGetCondition(ConditionType.MoveSpeed, out float value))
        {
            MovementSpeed = value;
        }
        else
        {
            Debug.LogError($"ConditionType MoveSpeed를 찾을 수 없습니다. 기본값으로 1.0f를 사용합니다.");
            MovementSpeed = 1.0f;
        }
    }
}
