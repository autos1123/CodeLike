using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private ConditionData conditionData;
    public EnemyCondition Condition { get; private set; }
    private EnemyStateMachine stateMachine;

    private void Awake()
    {
        Condition = new EnemyCondition(conditionData);
        stateMachine = new EnemyStateMachine(this);
    }
}
