using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerController:BaseController
{
    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundRayOffset = 0.1f;

    private PlayerInputHandler inputHandler;
    private BoxCollider col;
    private bool isGrounded;

    public PlayerStateMachine stateMachine { get; private set; }
    public bool IsAttacking { get; private set; }

    private PlayerCondition condition;
    public PlayerCondition PlayerCondition => condition;
    public Transform VisualTransform; // Visual 회전용
    public float VisualRotateSpeed = 10f; // 부드러운 회전 속도

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
        if(!isInitialized)
            return;

        if(ViewManager.Instance.IsTransitioning)
            return;
        // **카메라 전환 중엔 아무 동작 하지 않음**
        if(ViewManager.Instance.IsTransitioning)
            return;

        UpdateGrounded();

        if(inputHandler.JumpPressed && isGrounded)
            Jump();
        if(inputHandler.AttackPressed)
            stateMachine.ChangeState(new PlayerAttackState(this, stateMachine));

        stateMachine.Update();
        inputHandler.ResetOneTimeInputs();
    }

    private void FixedUpdate()
    {
        
        if(!isInitialized)
            return;
        // **카메라 전환 중엔 물리 업데이트도 스킵**
        if(ViewManager.Instance.IsTransitioning)
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

        if(hit != isGrounded)
            isGrounded = hit;

        Debug.DrawRay(origin, Vector3.down * distance, hit ? Color.green : Color.red);
    }

    public Vector3 Move(Vector2 input)
    {
        // MoveState 내부에서도 안전하게 무시 가능
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
    public void Attack()
    {
        Collider[] hitColliders = GetTargetColliders(LayerMask.GetMask("Enemy"));

        foreach(var hitCollider in hitColliders)
        {
            if(hitCollider.TryGetComponent(out IDamagable Enemy))
            {
                if(!data.TryGetCondition(ConditionType.AttackPower, out float power))
                {
                    power = 0.0f;
                }

                // 적에게 피해를 입히는 로직
                Enemy.GetDamaged(power);
            }
        }
    }

    private void Jump()
    {
        if(condition == null)
        {
            Debug.LogError("condition 이 null 입니다.");
        }
        if(_Rigidbody == null)
        {
            Debug.LogError("_Rigidbody 가 null 입니다.");
        }
        if(ViewManager.Instance == null)
        {
            Debug.LogError("ViewManager.Instance 가 null 입니다.");
        }
        float force = condition.GetValue(ConditionType.JumpPower);
        Vector3 v = _Rigidbody.velocity;
        if(ViewManager.Instance.CurrentViewMode == ViewModeType.View2D)
            _Rigidbody.velocity = new Vector3(v.x, 0f, 0f);
        else
            _Rigidbody.velocity = new Vector3(v.x, 0f, v.z);

        _Rigidbody.AddForce(Vector3.up * force, ForceMode.Impulse);
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
}
