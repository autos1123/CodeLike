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
        Debug.Log("Idle 진입!");
        Debug.Log(Player.InputHandler.JumpPressed);
        StartAnimation(Player.AnimationData.IdleParameterHash);
    }

    public override void StateExit()
    {
        base.StateExit();
        StopAnimation(Player.AnimationData.IdleParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        Vector2 move = stateMachine.Player.InputHandler.MoveInput;
        if(move != Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.MoveState); 
        }
        /// 점프 입력
        if(Player.InputHandler.JumpPressed && Player.IsGrounded)
        {
            Debug.Log(">> PlayerIdleState: 점프 입력 감지!");
            stateMachine.ChangeState(stateMachine.JumpState);
        }
        if(Player.InputHandler.AttackPressed)
        {
            stateMachine.ChangeState(stateMachine.Attack1State);
        }
        if(Player.InputHandler.SkillXPressed)
        {
            stateMachine.SkillState.SetSkill(Skillinput.X);
            stateMachine.ChangeState(stateMachine.SkillState);
            return;
        }
        if(Player.InputHandler.SkillCPressed)
        {
            stateMachine.SkillState.SetSkill(Skillinput.C);
            stateMachine.ChangeState(stateMachine.SkillState);
            return;
        }

    }

    public override void StatePhysicsUpdate() 
    {
        base.StatePhysicsUpdate();
    }
}
