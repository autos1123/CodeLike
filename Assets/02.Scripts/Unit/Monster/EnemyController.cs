using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyController:BaseController
{

    public EnemyStateMachine StateMachine { get; private set; }
    public EnemyAnimationData AnimationData { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Vector3 patrolPivot { get; private set; } = Vector3.zero;
    public GameObject Player { get; private set; } // 플레이어 오브젝트

    private GameObject hpBar;
    public PoolingHPBar HpUI => hpBar.GetComponent<PoolingHPBar>();

    // 게임 모드에 따라 상태를 변경하기 위한 필드
    private Vector3 destinationTmp; // NavMeshAgent의 목적지 저장
    private float agentSpeedTmp; // NavMeshAgent의 속도 저장

    public Room room {  get; private set; }

    protected override void OnEnable()
    {
        base.OnEnable();
        DestinyManager.Instance.onDestinyChange += HandleDestinyChange;//운명변경 이벤트 연결
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if(PoolManager.HasInstance && hpBar != null)
            PoolManager.Instance.ReturnObject(hpBar.GetComponent<IPoolObject>());

        if(DestinyManager.HasInstance)
            DestinyManager.Instance.onDestinyChange -= HandleDestinyChange;//운명변경 이벤트 연결해제
    }

    protected override void Awake()
    {
        base.Awake();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshAgent.speed = 0;
        NavMeshAgent.updateRotation = false; // NavMeshAgent가 회전을 처리하지 않도록 설정
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

    protected override void OnDrawGizmosSelected()
    {
        if(Application.isPlaying && isInitialized)
        {
            // 적의 순찰 범위를 시각적으로 표시
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(patrolPivot, Condition.GetTotalCurrentValue(ConditionType.PatrolRange));
            // 적의 추적 범위를 시각적으로 표시
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, Condition.GetTotalCurrentValue(ConditionType.ChaseRange));
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        Player = GameManager.Instance.Player; // 플레이어 오브젝트 초기화

        // Controller 초기화
        AnimationData = new EnemyAnimationData();
        StateMachine = new EnemyStateMachine(this);
        SetEnemyState();

        // 체력 UI 초기화
        hpBar = PoolManager.Instance.GetObject(PoolType.hpBar);
        hpBar.transform.SetParent(transform);
        hpBar.transform.localPosition = Vector3.zero + Vector3.up * 2f; // HP Bar 위치 조정
        HpBarUpdate();
        Condition.statModifiers[ConditionType.HP] += HpBarUpdate; // 체력 변화시 UI 업데이트

        room = GetComponentInParent<Room>();

        isInitialized = true;
    }

    public void HpBarUpdate()
    {
        HpUI.HpBarUpdate(Condition.GetConditionRatio(ConditionType.HP));
    }

    public override void Hit()
    {
        StateMachine.ChangeState(EnemyStateType.Hit); // 데미지를 받았을 때 상태 변경
    }

    public override void Die()
    {
        StateMachine.ChangeState(EnemyStateType.Die);
    }

    /// <summary>
    /// 적의 상태를 설정하는 추상 메서드
    /// 각 적 유형에 따라 사용 될 상태를 추가
    /// </summary>
    protected abstract void SetEnemyState();

    /// <summary>
    /// 적의 공격 액션을 수행하는 메서드
    /// </summary>
    public abstract void AttackAction();

    /// <summary>
    /// 미리 캐싱한 플레이어가 추적 범위에 들어왔는지 확인하는 메서드
    /// </summary>
    /// <returns></returns>
    public virtual bool IsInRange(ConditionType rangeType)
    {
        if(Player.GetComponent<PlayerController>().Condition.IsDied)
            return false; // 플레이어가 죽은 경우 추적하지 않음

        Vector3 targetPos = Player.transform.position;
        Vector3 curPos = transform.position;

        if(ViewManager.Instance.CurrentViewMode == ViewModeType.View2D)
        {
            // 2D인 경우 x축과 y축만 고려하여 거리 계산
            targetPos.z = 0;
            curPos.z = 0;
        }

        float playerDistanceSqr = (targetPos - curPos).sqrMagnitude;
        float range = Condition.GetTotalCurrentValue(rangeType);

        return playerDistanceSqr <= range * range;
    }

    protected override IEnumerator WaitForDataLoad()
    {
        yield return new WaitUntil(() => PoolManager.Instance.IsInitialized && GameManager.HasInstance);
        Initialize();
    }

    protected override void SetCharacterPauseMode(bool isPlaying)
    {
        if(StateMachine.CurrentStateType == EnemyStateType.Die)
            return; // 죽은 상태에서는 일시정지 모드 변경을 하지 않음

        base.SetCharacterPauseMode(isPlaying);

        if(!isPlaying)
        {
            destinationTmp = NavMeshAgent.destination;
            agentSpeedTmp = NavMeshAgent.speed; // NavMeshAgent 속도 저장
            NavMeshAgent.speed = 0; // NavMeshAgent 속도 0으로 설정
        }
        else
        {
            
            NavMeshAgent.destination = destinationTmp; // NavMeshAgent 목적지 복원
            NavMeshAgent.speed = agentSpeedTmp; // NavMeshAgent 속도 복원
        }
    }

    /// <summary>
    /// 운명 변경이벤트 발생시 실행할 함수
    /// </summary>
    /// <param name="data"></param>
    void HandleDestinyChange(DestinyData data, int i)
    {
        DestinyEffectData positiveEffect = TableManager.Instance.GetTable<DestinyEffectDataTable>().GetDataByID(data.PositiveEffectDataID);
        DestinyEffectData negativeEffect = TableManager.Instance.GetTable<DestinyEffectDataTable>().GetDataByID(data.NegativeEffectDataID);


        if(positiveEffect.effectedTarget == EffectedTarget.Enemy)
        {
            Condition.ChangeModifierValue(positiveEffect.conditionType, ModifierType.BuffEnhance, positiveEffect.value * i); // 추후에 운명에 의한 증가량 추가
        }

        if(negativeEffect.effectedTarget == EffectedTarget.Enemy)
        {
            Condition.ChangeModifierValue(negativeEffect.conditionType, ModifierType.BuffEnhance, negativeEffect.value * i); // 추후에 운명에 의한 증가량 추가
        }

    }
}
