using UnityEngine;

/// <summary>
/// 플레이어 이동 상태
/// </summary>
public class PlayerMoveState:PlayerBaseState
{
    private float inputDelay = 0f; // 입력 지연 시간
    private float inputDelayTime = 0.1f; // 입력 지연 시간 설정
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    
    public override void StateEnter()
    {
        base.StateEnter();
        StartAnimation(player.AnimationData.MoveParameterHash);
    }

    public override void StateExit()
    {
        base.StateExit();
        StopAnimation(player.AnimationData.MoveParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        Vector2 move = player.InputHandler.MoveInput;

        if(move != Vector2.zero)
        {
            inputDelay = 0f;
            PlayerLookAt();
        }
        else
        {
            inputDelay += Time.deltaTime;
            if(inputDelay >= inputDelayTime)
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
        }

        if(player.InputHandler.JumpPressed && player.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.JumpState);
        }


        if(player.InputHandler.AttackPressed)
            stateMachine.ChangeState(stateMachine.AttackState);
    }

    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
        Move(player.InputHandler.MoveInput);
    }
}
