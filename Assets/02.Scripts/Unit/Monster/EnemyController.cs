using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyController:BaseController
{
    private EnemyStateMachine stateMachine;
    [SerializeField] private float rotDamping;

    public EnemyCondition Condition { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public float RotationDamping => rotDamping;
    public Vector3 patrolPivot { get; private set; } = Vector3.zero;

    private GameObject hpBar;
    public PoolingHPBar HpUI => hpBar.GetComponent<PoolingHPBar>();

    private void OnEnable()
    {
        StartCoroutine(WaitForDataLoad());
    }

    private void OnDisable()
    {
        PoolManager.Instance.ReturnObject(hpBar.GetComponent<IPoolObject>());
    }

    protected override void Awake()
    {
        base.Awake();
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected override void Start()
    {
    }

    protected virtual void Update()
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

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if(Application.isPlaying && isInitialized)
        {
            float patrolRange;
            float chaseRange;

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
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        Condition = new EnemyCondition(data);
        stateMachine = new EnemyStateMachine(this);
        isInitialized = true;

        hpBar = PoolManager.Instance.GetObject(PoolType.hpBar);
        hpBar.transform.SetParent(transform);
        hpBar.transform.localPosition = Vector3.zero + Vector3.up * 2f; // HP Bar 위치 조정
        HpUI.HpBarUpdate(Condition.GetCurrentHpRatio());
    }

    public override bool GetDamaged(float damage)
    {
        if(!Condition.GetDamaged(damage))
        {
            // 몬스터 사망
            // 사망 이펙트 재생
            HpUI.HpBarUpdate(Condition.GetCurrentHpRatio());
            Invoke(nameof(EnemyDie), 0.1f);
            return false;
        }

        return true;
    }

    private void EnemyDie()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 적의 공격 액션을 수행하는 메서드
    /// </summary>
    public abstract void AttackAction();

    protected override IEnumerator WaitForDataLoad()
    {
        yield return new WaitUntil(() => GameManager.Instance.TableManager.loadComplete && PoolManager.Instance.IsInitialized);
        Initialize();
    }
}
