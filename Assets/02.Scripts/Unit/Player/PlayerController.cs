using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

/// <summary>
/// 플레이어 이동, 공격, 점프, 데미지 처리 및 FSM 관리
/// </summary>
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerController:BaseController<PlayerCondition>
{
    public PlayerInputHandler InputHandler { get; private set; }
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerAnimationData AnimationData { get; private set; } 

    public PlayerActiveItemController ActiveItemController { get; private set; }

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundRayOffset = 0.3f;

    private bool isGrounded;

    [Header("Visual Settings")]
    public Transform VisualTransform;
    public float VisualRotateSpeed = 10f;

    protected override void Awake()
    {
        base.Awake();
        InputHandler = GetComponent<PlayerInputHandler>();
        ActiveItemController = GetComponent<PlayerActiveItemController>();
        _Rigidbody.freezeRotation = true;
    }

    private void Update()
    {
        if(!isInitialized || !isPlaying)
            return;

        UpdateGrounded();

        // 점프 입력 처리
        if(InputHandler.JumpPressed && isGrounded)
            stateMachine.ChangeState(new PlayerJumpState(stateMachine));

        // 공격 입력 처리
        if(InputHandler.AttackPressed)
            stateMachine.ChangeState(new PlayerAttackState(stateMachine));

        // 현재 상태 업데이트
        stateMachine.Update();

        // 일회성 입력 리셋
        InputHandler.ResetOneTimeInputs();
    }

    private void FixedUpdate()
    {
        if(!isInitialized || !isPlaying)
            return;

        stateMachine.PhysicsUpdate();
    }

    /// <summary>
    /// 바닥 체크 (Raycast)
    /// </summary>
    private void UpdateGrounded()
    {
        Vector3 origin = col.bounds.center;
        float distance = col.bounds.extents.y + groundRayOffset;
        bool hit = Physics.Raycast(origin, Vector3.down, distance, groundLayer, QueryTriggerInteraction.Ignore);
        isGrounded = hit;
        Debug.DrawRay(origin, Vector3.down * distance, hit ? Color.green : Color.red);
    }
    /// <summary>
    /// 플레이어의 근접 공격 처리
    /// 주변 Enemy Layer 대상 충돌 검사 후 데미지 적용
    /// </summary>
    public void Attack()
    {
        Collider[] hitColliders = GetTargetColliders(LayerMask.GetMask("Enemy"));

        foreach(var hitCollider in hitColliders)
        {
            if(hitCollider.TryGetComponent(out IDamagable enemy))
            {
                enemy.GetDamaged(Condition.GetValue(ConditionType.AttackPower));
            }
        }
    }

    protected override void Die()
    {
        stateMachine.ChangeState(stateMachine.DeadState);
    }

    /// <summary>
    /// FSM 및 PlayerCondition 초기화
    /// </summary>
    protected override void Initialize()
    {
        base.Initialize();

        Condition = new PlayerCondition(InitConditionData());
        AnimationData = new PlayerAnimationData();
        stateMachine = new PlayerStateMachine(this);
        UIManager.Instance.ShowUI<HUD>();
        isInitialized = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.Instance.onDestinyChange += HandleDestinyChange;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if(GameManager.HasInstance)
            GameManager.Instance.onDestinyChange -= HandleDestinyChange;
    }

    /// <summary>
    /// 운명 변경이벤트 발생시 실행할 함수
    /// </summary>
    /// <param name="data"></param>
    void HandleDestinyChange(DestinyData data)
    {
        DestinyEffectData positiveEffect = TableManager.Instance.GetTable<DestinyEffectDataTable>().GetDataByID(data.PositiveEffectDataID);
        DestinyEffectData negativeEffect = TableManager.Instance.GetTable<DestinyEffectDataTable>().GetDataByID(data.NegativeEffectDataID);


        if(positiveEffect.effectedTarget == EffectedTarget.Player)
        {
            Condition.ChangeModifierValue(positiveEffect.conditionType, ModifierType.BuffEnhance, positiveEffect.value); // 추후에 운명에 의한 증가량 추가
        }

        if(negativeEffect.effectedTarget == EffectedTarget.Player)
        {
            Condition.ChangeModifierValue(negativeEffect.conditionType, ModifierType.BuffEnhance, negativeEffect.value); // 추후에 운명에 의한 증가량 추가
        }

    }

    public override void OnViewChange(ViewModeType viewMode)
    {
        base.OnViewChange(viewMode);
        if(viewMode == ViewModeType.View2D)
        {
            col.excludeLayers = LayerMask.GetMask("Enemy"); // 2D 모드에서는 Enemy 레이어 제외
        }
        else if(viewMode == ViewModeType.View3D)
        {
            col.excludeLayers = 0;
        }
    }
}
