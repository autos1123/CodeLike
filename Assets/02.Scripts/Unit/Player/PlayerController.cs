using UnityEngine;

/// <summary>
/// 플레이어 이동 및 상태(FSM), 스탯 관리 연동 컨트롤러 (엑셀 기반 SO 사용)
/// </summary>
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController:MonoBehaviour
{
    [Header("ConditionData SO (엑셀 기반 SO 연결)")]
    [SerializeField] private ConditionData conditionData;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;

    private PlayerInputHandler inputHandler;
    private Rigidbody rb;

    private bool isGrounded;

    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerCondition condition { get; private set; }

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        rb = GetComponent<Rigidbody>();

        // FSM 생성
        stateMachine = new PlayerStateMachine(this);

        // PlayerCondition 생성 및 초기화
        condition = new PlayerCondition(conditionData);
        condition.Init(this, stateMachine);

        Debug.Log("PlayerController 초기화 완료");
    }

    private void Update()
    {
        isGrounded = IsGrounded();

        // 점프 입력 처리
        if(inputHandler.JumpPressed && isGrounded)
        {
            Jump();
        }

        stateMachine.Update();
        inputHandler.ResetOneTimeInputs();
    }

    private void FixedUpdate()
    {
        stateMachine.PhysicsUpdate();
    }

    /// <summary>
    /// ViewMode에 따라 이동 방향 계산 및 속도 적용
    /// FSM 내에서 호출됨
    /// </summary>
    public Vector3 Move(Vector2 input)
    {
        float moveSpeed = condition.GetValue(ConditionType.MoveSpeed);

        Vector3 moveDirection;

        if(ViewManager.Instance.CurrentViewMode == ViewModeType.View2D)
        {
            moveDirection = new Vector3(input.x, 0f, 0f);
        }
        else
        {
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            moveDirection = camRight * input.x + camForward * input.y;
        }

        // 속도 적용
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;

        return moveDirection;
    }

    /// <summary>
    /// ViewMode에 따라 점프 처리 (엑셀 SO 기반 jumpForce 사용)
    /// </summary>
    private void Jump()
    {
        float jumpForce = condition.GetValue(ConditionType.JumpPower);
        Vector3 currentVelocity = rb.velocity;

        if(ViewManager.Instance.CurrentViewMode == ViewModeType.View2D)
        {
            rb.velocity = new Vector3(currentVelocity.x, 0f, 0f);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        else
        {
            rb.velocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// 바닥 감지 (Raycast 사용)
    /// </summary>
    private bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        Ray ray = new Ray(origin, Vector3.down);

        Debug.DrawRay(origin, Vector3.down * 0.3f, Color.red);

        return Physics.Raycast(ray, 0.3f, groundLayer);
    }

    public PlayerInputHandler Input => inputHandler;
}
