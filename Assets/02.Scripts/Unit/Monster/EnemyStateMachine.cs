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

        MovementSpeed = Enemy.Condition.Data.GetCondition(ConditionType.MoveSpeed).Value;
    }
}
