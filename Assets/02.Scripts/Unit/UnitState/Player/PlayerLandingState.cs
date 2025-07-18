using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandingState :PlayerBaseState
{
    float landingTime = 0.3f; // 착지 애니메이션 시간 (수정)
    float timer = 0f;
    public PlayerLandingState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    public override void StateEnter()
    {
        base.StateEnter();
        timer = 0f;
        StartAnimation(Player.AnimationData.LandingParameterHash);
    }
    public override void StateExit()
    {
        base.StateExit();
        Debug.Log("Landing 상태 종료");
        StopAnimation(Player.AnimationData.LandingParameterHash);
    }
    public override void StateUpdate()
    {
        base.StateUpdate();

        timer += Time.deltaTime;
        if(timer > landingTime)
        {
            if(Player.InputHandler.MoveInput.magnitude > 0.1f)
                stateMachine.ChangeState(stateMachine.MoveState);
            else
                stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
        // Handle physics-related updates if necessary
    }
}
