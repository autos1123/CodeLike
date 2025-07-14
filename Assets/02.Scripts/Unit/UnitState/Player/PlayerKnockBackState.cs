using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockBackState:PlayerBaseState
{
    private float knockBackDuration = 0.5f;
    private float startTime;
    public PlayerKnockBackState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    public override void StateEnter()
    {
        base.StateEnter();
        player._Animator.SetTrigger("isKnockBack");
        startTime = Time.time;
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        if(Time.time - startTime >= knockBackDuration)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
    }

    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
    }
}
