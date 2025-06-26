using UnityEngine;

/// <summary>
/// 플레이어 이동과 상태를 제어하는 컴포넌트
/// ViewMode (2D / 3D)에 따라 이동 방향과 점프 방식이 달라짐
/// </summary>
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController:MonoBehaviour
{
    private PlayerInputHandler inputHandler;
    private Rigidbody rb;

    [Header("이동 속도")]
    public float moveSpeed = 5f;

    [Header("점프 설정")]
    public float jumpForce = 7f;
    public float groundRayLength = 0.3f;         // 바닥 감지용 Ray 길이
    public LayerMask groundLayer;                // 바닥 판정할 레이어

    private bool isGrounded;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        isGrounded = IsGrounded();

        // 점프 입력 처리
        if(inputHandler.JumpPressed && isGrounded)
        {
            Jump();
        }

        inputHandler.ResetOneTimeInputs(); // 일회성 입력 초기화
    }

    private void FixedUpdate()
    {
        // 이동 처리 (물리 기반)
        Vector2 moveInput = inputHandler.MoveInput;

        Vector3 moveDir = Move(moveInput);
        Vector3 velocity = moveDir * moveSpeed;
        velocity.y = rb.velocity.y;

        rb.velocity = velocity;
    }

    /// <summary>
    /// ViewMode에 따라 이동 방향 계산
    /// </summary>
    public Vector3 Move(Vector2 input)
    {
        if(ViewManager.Instance.CurrentViewMode == ViewModeType.View2D)
        {
            return new Vector3(input.x, 0f, 0f); // X축만 이동 (Z 고정)
        }
        else
        {
            // 3D: 카메라 기준으로 방향 계산
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            return camRight * input.x + camForward * input.y;
        }
    }

    /// <summary>
    /// ViewMode에 따라 점프 처리
    /// </summary>
    private void Jump()
    {
        Vector3 currentVelocity = rb.velocity;

        if(ViewManager.Instance.CurrentViewMode == ViewModeType.View2D)
        {
            // Y축 점프, Z 고정
            rb.velocity = new Vector3(currentVelocity.x, 0f, 0f);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        else
        {
            // Y축 점프 (XZ 이동 유지)
            rb.velocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// 바닥 감지 (Raycast 사용)
    /// </summary>
    private bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f; // 살짝 위에서 쏨
        Ray ray = new Ray(origin, Vector3.down);

        Debug.DrawRay(origin, Vector3.down * groundRayLength, Color.red); // 디버그 시각화

        return Physics.Raycast(ray, groundRayLength, groundLayer);
    }

    /// <summary>
    /// 입력 핸들러 접근자
    /// </summary>
    public PlayerInputHandler Input => inputHandler;
}
