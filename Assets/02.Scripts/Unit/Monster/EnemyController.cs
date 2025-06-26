using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController:MonoBehaviour
{
    private EnemyStateMachine stateMachine;
    [SerializeField] private ConditionData data;
    [SerializeField] private float rotDamping;

    public EnemyCondition Condition { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public float RotationDamping => rotDamping;
    public ConditionData Data => data;

    private void Start()
    {
        Condition = new EnemyCondition(data);
        stateMachine = new EnemyStateMachine(this);
        Rigidbody = GetComponent<Rigidbody>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }
}
