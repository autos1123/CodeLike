using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 입력을 처리하고, ViewMode(2D/3D)에 따라 분기하여 방향 및 행동을 전달하는 클래스
/// </summary>
public class PlayerInputHandler:MonoBehaviour
{
    private PlayerInputActions inputActions;

    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool AttackPressed { get; private set; }

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        // 입력 이벤트 등록
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Jump.performed += ctx => JumpPressed = true;
        inputActions.Player.Attack.performed += ctx => AttackPressed = true;
    }

    private void OnEnable() => inputActions.Player.Enable();
    private void OnDisable() => inputActions.Player.Disable();

    private void Update()
    {
        // ViewMode에 따라 입력 해석 분기
        if(ViewManager.Instance.CurrentViewMode == ViewModeType.View2D)
        {
            // 2D는 x축만 사용
            MoveInput = new Vector2(MoveInput.x, 0f);
        }
        else
        {
            // 3D는 x, y 그대로 유지
            // 필요하다면 카메라 기준으로 변환도 가능
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void ResetOneTimeInputs()
    {
        JumpPressed = false;
        AttackPressed = false;
    }
}
