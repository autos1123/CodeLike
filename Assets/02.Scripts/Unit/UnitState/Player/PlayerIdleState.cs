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

        StopAnimation(player.AnimationData.MoveParameterHash);
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        var move = stateMachine.Player.InputHandler.MoveInput;
        if(move.magnitude > 0.1f)
        {
            stateMachine.ChangeState(stateMachine.MoveState); 
        }
        /// 점프 입력
        if(player.InputHandler.JumpPressed && player.IsGrounded)
            stateMachine.ChangeState(stateMachine.JumpState);

        // 공격 입력
        if(player.InputHandler.AttackPressed)
            stateMachine.ChangeState(stateMachine.AttackState);
    }

    public override void StatePhysicsUpdate() 
    {
        base.StatePhysicsUpdate();
    }
}
