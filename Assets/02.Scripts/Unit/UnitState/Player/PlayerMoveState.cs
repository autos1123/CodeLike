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
        if(player.IsGrounded)
            StartAnimation(player.AnimationData.MoveParameterHash);
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        Vector2 move = player.InputHandler.MoveInput;

        if(move.magnitude > 0.1f)
        {
            PlayerLookAt();
        }
        else
        {
            StopAnimation(player.AnimationData.MoveParameterHash);
            stateMachine.ChangeState(stateMachine.IdleState);
        }

        if(player.InputHandler.JumpPressed && player.IsGrounded)
        {
            StopAnimation(player.AnimationData.MoveParameterHash);
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
