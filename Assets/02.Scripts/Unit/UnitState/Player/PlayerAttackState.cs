public class PlayerAttackState:PlayerBaseState
{
    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();
        player._Animator.SetTrigger("attack");
        stateMachine.ChangeState(stateMachine.IdleState);
    }
}
