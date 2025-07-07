using UnityEngine;

/// <summary>
/// 플레이어의 기본 대기 상태
/// </summary>
public class PlayerIdleState:PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        Debug.Log("Idle 상태 진입");
        StartAnimation(Animator.StringToHash("isMoving")); // BaseState 함수 활용 (해당 함수 없으면 직접 _Animator 접근도 OK)
        // 혹은: stateMachine.Player._Animator.SetBool("isMoving", false);
    }

    public override void StateExit()
    {
        Debug.Log("Idle 상태 종료");
    }

    public override void StateUpdate()
    {
        var move = stateMachine.Player.Input.MoveInput;
        if(move.magnitude > 0.1f)
        {
            stateMachine.ChangeState(stateMachine.MoveState); // 상태 객체 재사용 패턴!
        }
    }

    public override void StatePhysicsUpdate() { }
}
