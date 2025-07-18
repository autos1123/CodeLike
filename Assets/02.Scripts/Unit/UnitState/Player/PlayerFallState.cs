using UnityEngine;

public class PlayerFallState:PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();
        Debug.Log("Fall 상태 진입");
        StartAnimation(Player.AnimationData.FallParameterHash);
    }

    public override void StateExit()
    {
        base.StateExit();
        Debug.Log("Fall 상태 종료");
        StopAnimation(Player.AnimationData.FallParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        if(Player.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.LandingState);
            return;
        }
    }

    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
        Move(Player.InputHandler.MoveInput);
    }
}
