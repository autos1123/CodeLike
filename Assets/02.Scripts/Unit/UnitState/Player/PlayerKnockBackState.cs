using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockBackState:PlayerBaseState
{
    public PlayerKnockBackState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    public override void StateEnter()
    {
        base.StateEnter();
        player._Animator.SetTrigger("isKnockBack");
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        stateMachine.ChangeState(stateMachine.IdleState);
    }

    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
    }
}
