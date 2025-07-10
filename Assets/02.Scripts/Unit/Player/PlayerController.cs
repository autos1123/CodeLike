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
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerAnimationData AnimationData { get; private set; }
    public PlayerActiveItemController ActiveItemController { get; private set; }
    public Room CurrentRoom { get; private set; }

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundRayOffset = 0.1f;

    public bool IsGrounded { get; private set; }

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
        StateMachine.Update();
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

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

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
        StateMachine.ChangeState(StateMachine.DeadState);
    }

    /// <summary>
    /// FSM 및 PlayerCondition 초기화
    /// </summary>
    protected override void Initialize()
    {
        base.Initialize();

        Condition = new PlayerCondition(InitConditionData());
        AnimationData = new PlayerAnimationData();
        StateMachine = new PlayerStateMachine(this);
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

    public void SetCurrentRoom(Room room)
    {
        if(CurrentRoom == room) return;

        CurrentRoom = room;
        Debug.Log($"[Player] 현재 Room 변경됨 → {room.Id}");

        // 미니맵 갱신 요청
        RequestMinimapUpdate();
    }

    private void RequestMinimapUpdate()
    {
        if(StageManager.Instance == null || StageManager.Instance.currentStage == null)
        {
            Debug.LogWarning("[Player] StageManager 또는 currentStage가 존재하지 않아 미니맵 업데이트를 건너뜁니다.");
            return;
        }

        var stage = StageManager.Instance.currentStage;
        var minimapData = MinimapBuilder.BuildFromStage(stage, stage.connections);

        foreach(var data in minimapData)
            data.isCurrent = data.roomID == CurrentRoom.Id;

        // UIManager가 UI를 모두 로드한 뒤에만 접근
        UIManager.Instance.OnAllUIReady(() =>
        {
            if(UIManager.Instance.TryGetUI<MinimapUI>(out var minimap))
            {
                minimap.GenerateMinimap(minimapData);
            }
            else
            {
                Debug.LogWarning("[Player] MinimapUI가 UIManager에 아직 등록되지 않았습니다.");
            }
        });
    }
}
