public class PlayerAttackState:PlayerBaseState
{
    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        player._Animator.SetTrigger("attack");
        stateMachine.ChangeState(stateMachine.IdleState);
    }

    public override void StateExit() { }
    public override void StateUpdate() { }
    public override void StatePhysicsUpdate() { }
}
