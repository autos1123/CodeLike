using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyController:BaseController
{
    private EnemyStateMachine stateMachine;
    [SerializeField] private float rotDamping;

    public EnemyAnimationData AnimationData { get; private set; }
    public EnemyCondition Condition { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public float RotationDamping => rotDamping;
    public Vector3 patrolPivot { get; private set; } = Vector3.zero;

    private GameObject hpBar;
    public PoolingHPBar HpUI => hpBar.GetComponent<PoolingHPBar>();

    // 게임 모드에 따라 상태를 변경하기 위한 필드
    private Vector3 destinationTmp; // NavMeshAgent의 목적지 저장

    protected override void OnDisable()
    {
        if(PoolManager.HasInstance)
            PoolManager.Instance.ReturnObject(hpBar.GetComponent<IPoolObject>());
    }

    protected override void Awake()
    {
        base.Awake();
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Update()
    {
        if(!isInitialized)
            return;

        if(!isPlaying)
            return;

        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        if(!isInitialized)
            return;

        if(!isPlaying)
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
        // Controller 초기화
        Condition = new EnemyCondition(data);
        AnimationData = new EnemyAnimationData();
        AnimationData.Initialize();
        stateMachine = new EnemyStateMachine(this);

        // 체력 UI 초기화
        hpBar = PoolManager.Instance.GetObject(PoolType.hpBar);
        hpBar.transform.SetParent(transform);
        hpBar.transform.localPosition = Vector3.zero + Vector3.up * 2f; // HP Bar 위치 조정
        HpUI.HpBarUpdate(Condition.GetCurrentHpRatio());

        isInitialized = true;
    }

    public override bool GetDamaged(float damage)
    {
        if(Condition.GetDamaged(damage))
        {
            // 몬스터 사망
            // 사망 이펙트 재생
            Invoke(nameof(EnemyDie), 0.1f);
            return false;
        }

        HpUI.HpBarUpdate(Condition.GetCurrentHpRatio());
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
        yield return new WaitUntil(() => TableManager.Instance.loadComplete && PoolManager.Instance.IsInitialized && GameManager.HasInstance);
        Initialize();
    }

    protected override void SetCharacterPauseMode(bool isPlaying)
    {
        base.SetCharacterPauseMode(isPlaying);

        if(!isPlaying)
        {
            destinationTmp = NavMeshAgent.destination;
            NavMeshAgent.isStopped = true; // NavMeshAgent 정지
        }
        else
        {
            NavMeshAgent.destination = destinationTmp; // NavMeshAgent 목적지 복원
            NavMeshAgent.isStopped = false; // NavMeshAgent 재개
        }
    }
}
