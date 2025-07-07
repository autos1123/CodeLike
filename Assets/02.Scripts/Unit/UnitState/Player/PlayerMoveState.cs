using UnityEngine;

/// <summary>
/// 플레이어 이동 상태
/// </summary>
public class PlayerMoveState:PlayerBaseState
{
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        Debug.Log("Move 상태 진입");
        StartAnimation(Animator.StringToHash("isMoving"));
    }

    public override void StateExit()
    {
        Debug.Log("Move 상태 종료");
        StopAnimation(Animator.StringToHash("isMoving"));
    }

    public override void StateUpdate()
    {
        Vector2 move = player.Input.MoveInput;

        if(move.magnitude > 0.1f)
        {
            // 3D 시점 회전
            if(viewMode == ViewModeType.View3D)
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
            // 2D 시점 회전
            else if(viewMode == ViewModeType.View2D)
            {
                if(move.x > 0.1f)
                    player.VisualTransform.localEulerAngles = new Vector3(0, 90, 0);
                else if(move.x < -0.1f)
                    player.VisualTransform.localEulerAngles = new Vector3(0, 270, 0);
            }
        }
        else
        {
            // 멈추면 Idle로 전환 (상태 객체 재사용)
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    public override void StatePhysicsUpdate()
    {
        Move(player.Input.MoveInput);
    }
}
