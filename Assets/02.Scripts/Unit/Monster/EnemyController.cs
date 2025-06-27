using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController:MonoBehaviour
{
    private EnemyStateMachine stateMachine;
    [SerializeField] private ConditionData data;
    [SerializeField] private float rotDamping;
    [SerializeField] private int ID;
    public EnemyCondition Condition { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public float RotationDamping => rotDamping;
    public ConditionData Data => data;
    public Vector3 patrolPivot { get; private set; } = Vector3.zero;

    private void Start()
    {
        data.InitDictionary();
        Condition = new EnemyCondition(data);
        stateMachine = new EnemyStateMachine(this);
        Rigidbody = GetComponent<Rigidbody>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        data = GameManager.Instance.TableManager.GetTable<ConditionDataTable>().GetDataByID(ID);
    }

    private void Update()
    {
        if(data.ID ==0)
        {
            data = GameManager.Instance.TableManager.GetTable<ConditionDataTable>().GetDataByID(ID);
        }
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.PhysicsUpdate();
    }

    public void SetPatrolPivot()
    {
        patrolPivot = transform.position;
    }

    private void OnDrawGizmos()
    {
        float patrolRange;
        float chaseRange;
        float attackRange;
        data.InitDictionary();

        if(data.TryGetCondition(ConditionType.PatrolRange, out patrolRange))
        {

            // 적의 순찰 범위를 시각적으로 표시
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(patrolPivot, patrolRange);
        }

        if(data.TryGetCondition(ConditionType.ChaseRange, out chaseRange))
        {
            // 적의 추적 범위를 시각적으로 표시
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }

        if(data.TryGetCondition(ConditionType.AttackRange, out attackRange))
        {
            // 적의 공격 범위를 시각적으로 표시
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
