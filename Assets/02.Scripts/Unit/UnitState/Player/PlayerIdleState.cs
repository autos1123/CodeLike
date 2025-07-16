using UnityEngine;

/// <summary>
/// 플레이어의 기본 대기 상태
/// </summary>
public class PlayerIdleState:PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();
        StartAnimation(Player.AnimationData.IdleParameterHash);
    }

    public override void StateExit()
    {
        base.StateExit();
        StopAnimation(Player.AnimationData.IdleParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        Vector2 move = stateMachine.Player.InputHandler.MoveInput;
        if(move != Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.MoveState); 
        }
        /// 점프 입력
        if(Player.InputHandler.JumpPressed && Player.IsGrounded)
            stateMachine.ChangeState(stateMachine.JumpState);

        // 공격 입력
        if(Player.InputHandler.AttackPressed)
            stateMachine.ChangeState(stateMachine.AttackState);
    }

    public override void StatePhysicsUpdate() 
    {
        base.StatePhysicsUpdate();
    }
}
