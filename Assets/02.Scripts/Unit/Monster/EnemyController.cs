using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyCondition Condition { get; private set; }
    private EnemyStateMachine stateMachine;

    private void Awake()
    {
        Condition = new EnemyCondition();
        stateMachine = new EnemyStateMachine(this);
    }
}
