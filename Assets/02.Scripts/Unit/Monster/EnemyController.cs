using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyController:BaseController<EnemyCondition>
{

    public EnemyStateMachine StateMachine { get; private set; }
    [SerializeField] private float rotDamping;

    public EnemyAnimationData AnimationData { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public float RotationDamping => rotDamping;
    public Vector3 patrolPivot { get; private set; } = Vector3.zero;

    private GameObject hpBar;
    public PoolingHPBar HpUI => hpBar.GetComponent<PoolingHPBar>();

    // 게임 모드에 따라 상태를 변경하기 위한 필드
    private Vector3 destinationTmp; // NavMeshAgent의 목적지 저장

    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.Instance.onDestinyChange += HandleDestinyChange;//운명변경 이벤트 연결
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if(PoolManager.HasInstance)
            PoolManager.Instance.ReturnObject(hpBar.GetComponent<IPoolObject>());

        if(GameManager.HasInstance)
            GameManager.Instance.onDestinyChange -= HandleDestinyChange;//운명변경 이벤트 연결해제
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

        StateMachine.Update();
    }

    private void FixedUpdate()
    {
        if(!isInitialized)
            return;

        if(!isPlaying)
            return;

        StateMachine.PhysicsUpdate();
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
            // 적의 순찰 범위를 시각적으로 표시
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(patrolPivot, Condition.GetValue(ConditionType.PatrolRange));
            // 적의 추적 범위를 시각적으로 표시
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, Condition.GetValue(ConditionType.ChaseRange));
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        // Controller 초기화
        Condition = new EnemyCondition(InitConditionData());
        AnimationData = new EnemyAnimationData();
        StateMachine = new EnemyStateMachine(this);

        // 체력 UI 초기화
        hpBar = PoolManager.Instance.GetObject(PoolType.hpBar);
        hpBar.transform.SetParent(transform);
        hpBar.transform.localPosition = Vector3.zero + Vector3.up * 2f; // HP Bar 위치 조정
        HpBarUpdate();
        Condition.statModifiers[ConditionType.HP] += HpBarUpdate; // 체력 변화시 UI 업데이트

        isInitialized = true;
    }

    public void HpBarUpdate()
    {
        HpUI.HpBarUpdate(Condition.GetConditionRatio(ConditionType.HP));
    }

    protected override void Die()
    {
        StateMachine.ChangeState(StateMachine.DieState);
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

    /// <summary>
    /// 운명 변경이벤트 발생시 실행할 함수
    /// </summary>
    /// <param name="data"></param>
    void HandleDestinyChange(DestinyData data)
    {
        DestinyEffectData positiveEffect = TableManager.Instance.GetTable<DestinyEffectDataTable>().GetDataByID(data.PositiveEffectDataID);
        DestinyEffectData negativeEffect = TableManager.Instance.GetTable<DestinyEffectDataTable>().GetDataByID(data.NegativeEffectDataID);


        if(positiveEffect.effectedTarget == EffectedTarget.Enemy)
        {
            Condition.ChangeModifierValue(positiveEffect.conditionType, ModifierType.BuffEnhance, positiveEffect.value); // 추후에 운명에 의한 증가량 추가
        }

        if(negativeEffect.effectedTarget == EffectedTarget.Enemy)
        {
            Condition.ChangeModifierValue(negativeEffect.conditionType, ModifierType.BuffEnhance, negativeEffect.value); // 추후에 운명에 의한 증가량 추가
        }

    }
}
