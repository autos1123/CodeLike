using UnityEngine;

/// <summary>
/// 플레이어 이동 상태
/// </summary>
public class PlayerMoveState:PlayerBaseState
{
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();
        StartAnimation(Animator.StringToHash("isMoving"));
    }

    public override void StateExit()
    {
        base.StateExit();
        StopAnimation(Animator.StringToHash("isMoving"));
    }

    public override void StateUpdate()
    {
        Vector2 move = player.InputHandler.MoveInput;

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
                    player.VisualTransform.rotation = Quaternion.Lerp(player.VisualTransform.rotation, targetRot, Time.deltaTime * player.VisualRotateSpeed);
                }
            }
            // 2D 시점 회전
            else if(viewMode == ViewModeType.View2D)
            {
                var camRight = Camera.main.transform.right; camRight.y = 0; camRight.Normalize();
                Vector3 moveDir = camRight * move.x;

                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                player.VisualTransform.rotation = targetRot;
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
        Move(player.InputHandler.MoveInput);
    }
}
