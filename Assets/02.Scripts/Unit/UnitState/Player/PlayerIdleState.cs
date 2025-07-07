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
        StopAnimation(Animator.StringToHash("isMoving"));
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateUpdate()
    {
        var move = stateMachine.Player.InputHandler.MoveInput;
        if(move.magnitude > 0.1f)
        {
            stateMachine.ChangeState(stateMachine.MoveState); // 상태 객체 재사용 패턴!
        }
    }

    public override void StatePhysicsUpdate() { }
}
