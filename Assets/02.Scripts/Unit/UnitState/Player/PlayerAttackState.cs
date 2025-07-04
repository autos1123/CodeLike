using UnityEngine;

public class PlayerAttackState:IUnitState
{
    private PlayerController player;
    private PlayerStateMachine stateMachine;

    public PlayerAttackState(PlayerController player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public void StateEnter()
    {
        player._Animator.SetTrigger("attack");

        // 즉시 Idle로 전환 (예시로 IdleState 사용)
        stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
    }

    public void StateExit()
    {
    }


    public void StateUpdate()
    {
        
    }

    public void StatePhysicsUpdate() { }
}

