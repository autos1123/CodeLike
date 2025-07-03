using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 이동, 공격, 점프, 데미지 처리 및 FSM 관리
/// </summary>
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerController:BaseController
{
    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundRayOffset = 0.3f;

    private PlayerInputHandler inputHandler;
    private BoxCollider col;
    private bool isGrounded;
    public bool IsGrounded => isGrounded;

    public PlayerStateMachine stateMachine { get; private set; }
    public bool IsAttacking { get; private set; }

    private PlayerCondition condition;
    public PlayerCondition PlayerCondition => condition;

    [Header("Visual Settings")]
    public Transform VisualTransform;
    public float VisualRotateSpeed = 10f;


    protected override void Awake()
    {
        base.Awake();
        inputHandler = GetComponent<PlayerInputHandler>();
        col = GetComponent<BoxCollider>();
        _Rigidbody.freezeRotation = true;
        stateMachine = new PlayerStateMachine(this);
    }

    protected override void Start()
    {
        base.Start();

    }

    

    private void Update()
    {
        if(inputHandler.TestDamageKeyPressed())
        {
            GetDamaged(10f);
        }



        if(!isInitialized || ViewManager.Instance.IsTransitioning)
            return;

        UpdateGrounded();

        // 점프 입력 처리
        if(inputHandler.JumpPressed && isGrounded)
            stateMachine.ChangeState(new PlayerJumpState(this, stateMachine));

        // 공격 입력 처리
        if(inputHandler.AttackPressed)
            stateMachine.ChangeState(new PlayerAttackState(this, stateMachine));

        // 현재 상태 업데이트
        stateMachine.Update();

        // 일회성 입력 리셋
        inputHandler.ResetOneTimeInputs();
    }

    private void FixedUpdate()
    {
        if(!isInitialized || ViewManager.Instance.IsTransitioning)
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
    /// 플레이어 이동 처리
    /// </summary>
    public Vector3 Move(Vector2 input)
    {
        if(ViewManager.Instance.IsTransitioning)
            return Vector3.zero;

        float speed = 0f;
        if(!data.TryGetCondition(ConditionType.MoveSpeed, out speed))
        {
            Debug.LogWarning("[PlayerController] MoveSpeed가 설정되어 있지 않습니다.");
        }

        Vector3 dir;
        if(ViewManager.Instance.CurrentViewMode == ViewModeType.View2D)
            dir = new Vector3(input.x, 0f, 0f);
        else
        {
            var f = Camera.main.transform.forward; f.y = 0; f.Normalize();
            var r = Camera.main.transform.right; r.y = 0; r.Normalize();
            dir = r * input.x + f * input.y;
        }

        Vector3 delta = dir * speed * Time.fixedDeltaTime;
        _Rigidbody.MovePosition(_Rigidbody.position + delta);
        return dir;
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
                if(!data.TryGetCondition(ConditionType.AttackPower, out float power))
                {
                    power = 0f;
                    Debug.LogWarning("[PlayerController] AttackPower 정보를 가져오지 못해 0으로 처리합니다.");
                }

                enemy.GetDamaged(power);
            }
        }
    }


    /// <summary>
    /// 데미지 처리 (방어력 적용 후 PlayerCondition에 전달)
    /// </summary>
    public override bool GetDamaged(float damage)
    {
        Debug.LogWarning($"[PlayerController] {damage} 데미지 받음 처리 중");
        float defense = 0f;
        if(!data.TryGetCondition(ConditionType.Defense, out defense))
        {
            Debug.LogWarning("[PlayerController] Defense 값이 없어 0으로 처리합니다.");
        }

        float reducedDamage = Mathf.Max(0, damage - defense);
        return PlayerCondition.TakenDamage(reducedDamage);
    }

    /// <summary>
    /// FSM 및 PlayerCondition 초기화
    /// </summary>
    protected override void Initialize()
    {
        base.Initialize();
        Debug.Log("[PlayerController] Initialize 호출");

        condition = new PlayerCondition(data);
        stateMachine = new PlayerStateMachine(this);
        condition.Init(this, stateMachine);

        isInitialized = true;
        Debug.Log("[PlayerController] Initialize 완료");
    }

    /// <summary>
    /// PlayerInputHandler 접근용 프로퍼티
    /// </summary>
    public PlayerInputHandler Input => inputHandler;

    
}
