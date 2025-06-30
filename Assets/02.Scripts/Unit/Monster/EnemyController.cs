using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController:BaseController
{
    private EnemyStateMachine stateMachine;
    [SerializeField] private float rotDamping;

    public EnemyCondition Condition { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public float RotationDamping => rotDamping;
    public Vector3 patrolPivot { get; private set; } = Vector3.zero;

    protected override void Awake()
    {
        base.Awake();
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if(!isInitialized)
            return;

        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        if(!isInitialized)
            return;

        stateMachine.PhysicsUpdate();
    }

    public void SetPatrolPivot()
    {
        patrolPivot = transform.position;
    }

    protected virtual void OnDrawGizmos()
    {
        if(Application.isPlaying && isInitialized)
        {
            float patrolRange;
            float chaseRange;
            float attackRange;

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

    protected override void Initialize()
    {
        base.Initialize();
        Condition = new EnemyCondition(data);
        stateMachine = new EnemyStateMachine(this);
        isInitialized = true;
    }

    /// <summary>
    /// 적의 공격 액션을 수행하는 메서드
    /// </summary>
    public virtual void AttackAction()
    {

    }
}
