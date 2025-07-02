using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 입력을 처리하고, ViewMode(2D/3D)에 따라 분기하지 않고 원본 입력을 유지.
/// 이동 방향 분기는 실제 이동 처리에서 적용.
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler:MonoBehaviour
{
    private PlayerInputActions inputActions;

    /// <summary>
    /// 이동 방향 입력 (Vector2)
    /// 2D: (좌우), 3D: (좌우 + 상하)
    /// </summary>
    public Vector2 MoveInput { get; private set; }

    /// <summary>
    /// Jump 버튼이 눌렸는지 여부 (한 프레임만 true)
    /// </summary>
    public bool JumpPressed { get; private set; }

    /// <summary>
    /// Attack 버튼이 눌렸는지 여부 (한 프레임만 true)
    /// </summary>
    public bool AttackPressed { get; private set; }

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        // 방향키, WASD 등의 방향 입력 등록
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        // 점프 및 공격: 한 프레임만 true로 처리
        inputActions.Player.Jump.performed += ctx => JumpPressed = true;
        inputActions.Player.Attack.performed += ctx => AttackPressed = true;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    /// <summary>
    /// 방향 입력 처리 (입력 중 또는 입력 취소 시 둘 다 호출됨)
    /// </summary>
    private void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("@@@@");
        MoveInput = context.ReadValue<Vector2>();
        // 입력이 취소되면 (0, 0)이 자동으로 들어옴
    }

    /// <summary>
    /// Jump, Attack 같은 일회성 입력값을 초기화
    /// 매 프레임 후 호출 필요
    /// </summary>
    public void ResetOneTimeInputs()
    {
        JumpPressed = false;
        AttackPressed = false;
    }

    public bool IsPressingDown()
    {
        return MoveInput.y < -0.5f;
    }
}
