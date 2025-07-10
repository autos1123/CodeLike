using System;
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

    /// <summary>
    /// 상호작용 키 입력 시 호출되는 이벤트
    /// </summary>
    public Action<InputAction.CallbackContext> OnInteraction;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        // 방향키, WASD 등의 방향 입력 등록
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        // 점프 및 공격: 한 프레임만 true로 처리
        inputActions.Player.Jump.performed += ctx => JumpPressed = true;
        inputActions.Player.Attack.performed += ctx => AttackPressed = true;
        inputActions.Player.ChangeView.performed += ctx => ViewManager.Instance.ToggleView();
        inputActions.Player.OpenOptions.performed += ctx => UIManager.Instance.ToggleUI<OptionBoard>();
        inputActions.Player.OpenInventory.performed += ctx => UIManager.Instance.ToggleUI<InventoryUI>();
        inputActions.Player.OpenStatus.performed += ctx => UIManager.Instance.ToggleUI<StatusBoard>();
        inputActions.Player.OpenMinmap.performed += ctx => UIManager.Instance.ToggleUI<MinimapUI>();
        inputActions.Player.UseXItem.performed += ctx => GameManager.Instance.Player.transform.GetComponent<PlayerActiveItemController>().UseItem(Skillinput.X);
        inputActions.Player.UseCitem.performed += ctx => GameManager.Instance.Player.transform.GetComponent<PlayerActiveItemController>().UseItem(Skillinput.C);
        // 상호작용 입력 (F키)
        inputActions.Player.Interaction.performed += OnInteraction;
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

    public bool TestDamageKeyPressed()
    {
        return Keyboard.current.hKey.wasPressedThisFrame;
    }
}
