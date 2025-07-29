using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

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

    private float staminaDrainPerSecond = 5f;

    public Action OnSkillInput;

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
    }

    private void Update()
    {
        if(!isInitialized || !isPlaying)
            return;

        UpdateGrounded();
        StateMachine.Update();
        InputHandler.ResetOneTimeInputs();

        if(ViewManager.Instance.CurrentViewMode == ViewModeType.View3D)
        {
            if(Condition.CurrentConditions[ConditionType.Stamina] > 0f)
            {
                Condition.CurrentConditions[ConditionType.Stamina] -= staminaDrainPerSecond * Time.deltaTime;
                Condition.CurrentConditions[ConditionType.Stamina] = Mathf.Max(0f, Condition.CurrentConditions[ConditionType.Stamina]);
                Condition.statModifiers[ConditionType.Stamina]?.Invoke();
            }
            else
            {
                ViewManager.Instance.SwitchView(ViewModeType.View2D);
            }
        }
        else
        {
            // 2D일 때만 리젠!
            Condition.RegenerateStamina();
        }
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
        Vector3 boxHalfExtents = new Vector3(extents.x - 0.1f, groundRayOffset * 0.5f, extents.z - 0.1f);

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
        SoundManager.Instance.PlayBGM(this.transform, SoundAddressbleName.Boss_Battle);

        // 인벤토리 초기화 
        Inventory inventory = GetComponent<Inventory>();
        if(inventory != null)
        {
            inventory.InitializeInventory(); // TableManager 준비될 때까지 대기 후 초기화
        }
        //UIManager.Instance.ShowUI<HUD>();
        isInitialized = true;
    }
}
