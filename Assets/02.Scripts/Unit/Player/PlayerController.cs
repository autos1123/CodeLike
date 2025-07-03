using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public Transform VisualTransform;
    public float VisualRotateSpeed = 10f;

    [Header("Interaction")]
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float interactableRange = 2.0f;

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

    private void OnEnable()
    {
        if(inputHandler != null)
        {
            inputHandler.OnInteraction += OnInteractableAction;
        }
    }

    private void OnDisable()
    {
        if(inputHandler != null)
        {
            inputHandler.OnInteraction -= OnInteractableAction;
        }
    }

    private void Update()
    {
        if(!isInitialized || ViewManager.Instance.IsTransitioning)
            return;

        UpdateGrounded();

        if(inputHandler.JumpPressed && isGrounded)
            stateMachine.ChangeState(new PlayerJumpState(this, stateMachine));

        if(inputHandler.AttackPressed)
            stateMachine.ChangeState(new PlayerAttackState(this, stateMachine));

        stateMachine.Update();
        inputHandler.ResetOneTimeInputs();
    }

    private void FixedUpdate()
    {
        if(!isInitialized || ViewManager.Instance.IsTransitioning)
            return;

        stateMachine.PhysicsUpdate();
    }

    private void UpdateGrounded()
    {
        Vector3 origin = col.bounds.center;
        float distance = col.bounds.extents.y + groundRayOffset;

        bool hit = Physics.Raycast(
            origin,
            Vector3.down,
            distance,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        isGrounded = hit;
        Debug.DrawRay(origin, Vector3.down * distance, hit ? Color.green : Color.red);
    }

    public Vector3 Move(Vector2 input)
    {
        if(ViewManager.Instance.IsTransitioning)
            return Vector3.zero;

        float speed = condition.GetValue(ConditionType.MoveSpeed);
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

    public override bool GetDamaged(float damage)
    {
        if(!condition.GetDamaged(damage))
        {
            // 플레이어 사망 처리
            return false;
        }

        return true;
    }

    public void Attack()
    {
        Collider[] hitColliders = GetTargetColliders(LayerMask.GetMask("Enemy"));

        foreach(var hitCollider in hitColliders)
        {
            if(hitCollider.TryGetComponent(out IDamagable Enemy))
            {
                if(!data.TryGetCondition(ConditionType.AttackPower, out float power))
                    power = 0.0f;

                Enemy.GetDamaged(power);
            }
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        Debug.Log("PlayerController Initialize 호출");
        condition = new PlayerCondition(data);
        stateMachine = new PlayerStateMachine(this);
        condition.Init(this, stateMachine);
        isInitialized = true;
        Debug.Log("PlayerController Initialize 완료, isInitialized = true");
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    public PlayerInputHandler Input => inputHandler;

    /// <summary>
    /// 상호작용 키 입력 시 호출되는 메서드
    /// 플레이어 중심으로 상호작용 오브젝트 탐색 후 상호작용 메서드 호출
    /// </summary>
    /// <param name="context"></param>
    public void OnInteractableAction(InputAction.CallbackContext context)
    {
        // 상호작용 오브젝트 탐색
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            interactableRange,
            interactableLayer
        );

        if(hitColliders.Length == 0)
            return;

        for(int i = 0; i < hitColliders.Length; i++)
        {
            if(hitColliders[i].TryGetComponent(out IInteractable interactable))
            {
                // 상호작용 메서드 호출
                interactable.Interact(gameObject);
                return; // 첫 번째 상호작용만 처리
            }
        }
    }
}
