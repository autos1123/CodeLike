using UnityEngine;

public class PlayerAttackState:PlayerBaseState
{
    private float attackDuration = 0.5f;
    private float startTime;

    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();
        player._Animator.SetTrigger("attack");
        startTime = Time.time;
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if(Time.time - startTime >= attackDuration)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
    }

    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
        Move(player.InputHandler.MoveInput); // 공격 중 이동 허용
    }
}
