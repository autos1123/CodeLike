using UnityEngine;

/// <summary>
/// 플레이어 이동 상태
/// </summary>
public class PlayerMoveState:IUnitState
{
    private PlayerController player;
    private PlayerStateMachine stateMachine;

    public PlayerMoveState(PlayerController player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public void StateEnter()
    {
        Debug.Log("Move 상태 진입");
        player._Animator.SetBool("isMoving", true);
    }

    public void StateExit()
    {
        Debug.Log("Move 상태 종료");
        player._Animator.SetBool("isMoving", false);
    }

    public void StateUpdate()
    {
        Debug.Log("VisualTransform Rotation: " + player.VisualTransform.rotation.eulerAngles);
        Vector2 move = player.Input.MoveInput;

        if(move.magnitude > 0.1f)
        {
            if(ViewManager.Instance.CurrentViewMode == ViewModeType.View3D)
            {
                var camForward = Camera.main.transform.forward; camForward.y = 0; camForward.Normalize();
                var camRight = Camera.main.transform.right; camRight.y = 0; camRight.Normalize();
                Vector3 moveDir = camRight * move.x + camForward * move.y;

                if(moveDir.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(moveDir);
                    player.VisualTransform.rotation = Quaternion.Slerp(
                        player.VisualTransform.rotation,
                        targetRot,
                        Time.deltaTime * player.VisualRotateSpeed
                    );
                }
            }
            else if(ViewManager.Instance.CurrentViewMode == ViewModeType.View2D)
            {
                // **딱 좌/우 입력만 처리**
                if(move.x > 0.1f)
                    player.VisualTransform.localEulerAngles = new Vector3(0, 90, 0);
                else if(move.x < -0.1f)
                    player.VisualTransform.localEulerAngles = new Vector3(0, 270, 0);


            }
        }
        else
        {
            // 멈추면 Idle로 전환
            stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
        }
    }



    public void StatePhysicsUpdate()
    {
        player.Move(player.Input.MoveInput);
    }
}
