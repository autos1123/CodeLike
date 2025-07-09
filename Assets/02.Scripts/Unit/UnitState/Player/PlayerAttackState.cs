public class PlayerAttackState:PlayerBaseState
{
    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();
        player._Animator.SetTrigger("attack");
        stateMachine.ChangeState(stateMachine.IdleState);
    }

    public override void StateExit()
    {
        base.StateExit();
    }
    
    public override void StateUpdate()
    {
        base.StateUpdate();
        if(player.InputHandler.MoveInput.magnitude < 0.1f)
            stateMachine.ChangeState(stateMachine.IdleState);

        if(player.InputHandler.JumpPressed && player.IsGrounded)
            stateMachine.ChangeState(stateMachine.JumpState);

        if(player.InputHandler.AttackPressed)
            stateMachine.ChangeState(stateMachine.AttackState);
    }
    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
    }
}
