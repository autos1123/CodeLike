using UnityEngine;

/// <summary>
/// 플레이어의 이동 및 상태를 제어하는 컴포넌트
/// </summary>
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController:MonoBehaviour
{
    private PlayerInputHandler inputHandler;
    private PlayerStateMachine stateMachine;

    public float moveSpeed = 5f;
    public Rigidbody2D rb2D;
    public Rigidbody rb3D;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        stateMachine = new PlayerStateMachine(this);
    }

    private void Update()
    {
        stateMachine.Update();

        // 입력 처리 후 일회성 입력 초기화
        inputHandler.ResetOneTimeInputs();
    }

    private void FixedUpdate()
    {
        stateMachine.PhysicsUpdate();
    }

    /// <summary>
    /// 실제 이동을 처리 (View에 따라 물리 방식 분리)
    /// </summary>
    public void Move(Vector2 direction)
    {
        if(ViewManager.Instance.CurrentViewMode == ViewModeType.View2D)
        {
            rb2D.velocity = new Vector2(direction.x * moveSpeed, rb2D.velocity.y);
        }
        else
        {
            Vector3 dir = new Vector3(direction.x, 0f, direction.y); // 3D는 Z축 이동
            rb3D.velocity = dir * moveSpeed + new Vector3(0f, rb3D.velocity.y, 0f);
        }
    }

    public PlayerInputHandler Input => inputHandler;
}
