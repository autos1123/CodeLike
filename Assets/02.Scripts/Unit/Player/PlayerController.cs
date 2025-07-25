using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// 플레이어 이동, 공격, 점프, 데미지 처리 및 FSM 관리
/// </summary>
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerController:BaseController
{
    public PlayerInputHandler InputHandler { get; private set; }
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerAnimationData AnimationData { get; private set; }
    public PlayerActiveItemController ActiveItemController { get; private set; }
    public Room CurrentRoom { get; private set; }

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundRayOffset = 0.1f;

    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float startTime;

    public bool IsGrounded { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        InputHandler = GetComponent<PlayerInputHandler>();
        ActiveItemController = GetComponent<PlayerActiveItemController>();
        _Rigidbody.freezeRotation = true;
        startTime = Time.time;
    }

    private void Update()
    {
        if(!isInitialized || !isPlaying)
            return;

        UpdateGrounded();
        StateMachine.Update();
        AttackCheck();
        InputHandler.ResetOneTimeInputs();
        
    }

    private void FixedUpdate()
    {
        if(!isInitialized || !isPlaying)
            return;

        StateMachine.PhysicsUpdate();
    }

    /// <summary>
    /// 바닥 체크 (Raycast)
    /// </summary>
    private void UpdateGrounded()
    {
        Vector3 center = col.bounds.center;
        Vector3 extents = col.bounds.extents;

        // 아래 방향으로 약간 이동한 박스 중심 위치
        Vector3 boxCenter = center + Vector3.down * (extents.y + groundRayOffset * 0.5f);

        // 박스 크기 (살짝 얇게 Y축 조정)
        Vector3 boxHalfExtents = new Vector3(extents.x, groundRayOffset * 0.5f, extents.z);

        bool isGrounded = Physics.OverlapBox(boxCenter, boxHalfExtents, Quaternion.identity, groundLayer).Length > 0;

        IsGrounded = isGrounded;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if(Application.isPlaying)
        {
            Vector3 center = col.bounds.center;
            Vector3 extents = col.bounds.extents;
            Vector3 boxCenter = center + Vector3.down * (extents.y + groundRayOffset * 0.5f);
            Vector3 boxHalfExtents = new Vector3(extents.x, groundRayOffset * 0.5f, extents.z);

            Gizmos.color = IsGrounded ? Color.blue : Color.black;
            Gizmos.DrawWireCube(boxCenter, boxHalfExtents * 2);
        }
    }

    /// <summary>
    /// 플레이어의 근접 공격 처리
    /// 주변 Enemy Layer 대상 충돌 검사 후 데미지 적용
    /// </summary>
    public void Attack()
    {
        Collider[] hitColliders = _CombatController.GetTargetColliders(LayerMask.GetMask("Enemy"));

        foreach(var hitCollider in hitColliders)
        {
            if(hitCollider.TryGetComponent(out IDamagable enemy))
            {
                enemy.GetDamaged(Condition.GetTotalCurrentValue(ConditionType.AttackPower));
            }
        }
    }
    public void AttackCheck()
    {
        if(InputHandler.AttackPressed && Time.time - startTime >= attackDuration)
        {
            startTime = Time.time;
            _Animator.SetTrigger(AnimationData.AttackParameterHash);
        }
    }

    public override void Hit()
    {
        StateMachine.ChangeState(StateMachine.KnockbackState);
    }

    public override void Die()
    {
        StateMachine.ChangeState(StateMachine.DeadState);
        
        UIManager.Instance.ShowConfirmPopup(
            "사망했습니다! 로비로 돌아갑니다.",
            onConfirm: () => {
                SceneManager.LoadScene("LobbyScene");
            },
            onCancel: null, 
            confirmText: "확인" 
            );
    }

    /// <summary>
    /// FSM 및 PlayerCondition 초기화
    /// </summary>
    protected override void Initialize()
    {
        base.Initialize();

        AnimationData = new PlayerAnimationData();
        StateMachine = new PlayerStateMachine(this);

        UIManager.Instance.ShowUI<HUD>();

        //임식 bgm 시작
        SoundManager.Instance.PlayBGM(GameManager.Instance.Player.transform, SoundAddressbleName.Boss_Battle);

        // 인벤토리 초기화 
        Inventory inventory = GetComponent<Inventory>();
        if(inventory != null)
        {
            inventory.InitializeInventory(); // TableManager 준비될 때까지 대기 후 초기화
        }
        //UIManager.Instance.ShowUI<HUD>();
        isInitialized = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        DestinyManager.Instance.onDestinyChange += HandleDestinyChange;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if(DestinyManager.HasInstance)
            DestinyManager.Instance.onDestinyChange -= HandleDestinyChange;
    }

    /// <summary>
    /// 운명 변경이벤트 발생시 실행할 함수
    /// </summary>
    /// <param name="data"></param>
    void HandleDestinyChange(DestinyData data, int i)
    {
        DestinyEffectData positiveEffect = TableManager.Instance.GetTable<DestinyEffectDataTable>().GetDataByID(data.PositiveEffectDataID);
        DestinyEffectData negativeEffect = TableManager.Instance.GetTable<DestinyEffectDataTable>().GetDataByID(data.NegativeEffectDataID);


        if(positiveEffect.effectedTarget == EffectedTarget.Player)
        {
            Condition.ChangeModifierValue(positiveEffect.conditionType, ModifierType.BuffEnhance, positiveEffect.value * i); // 추후에 운명에 의한 증가량 추가
        }

        if(negativeEffect.effectedTarget == EffectedTarget.Player)
        {
            Condition.ChangeModifierValue(negativeEffect.conditionType, ModifierType.BuffEnhance, negativeEffect.value * i); // 추후에 운명에 의한 증가량 추가
        }

    }
}
