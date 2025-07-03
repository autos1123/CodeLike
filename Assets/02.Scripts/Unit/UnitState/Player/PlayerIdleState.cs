using UnityEngine;

/// <summary>
/// 플레이어의 기본 대기 상태
/// </summary>
public class PlayerIdleState:IUnitState
{
    private PlayerController player;
    private PlayerStateMachine stateMachine;

    public PlayerIdleState(PlayerController player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public void StateEnter()
    {
        Debug.Log("Idle 상태 진입");
        player._Animator.SetBool("isMoving", false);
    }

    public void StateExit()
    {
        Debug.Log("Idle 상태 종료");
    }

    public void StateUpdate()
    {
        var move = player.Input.MoveInput;
        if(move.magnitude > 0.1f)
        {
            stateMachine.ChangeState(new PlayerMoveState(player, stateMachine));
        }
    }

    public void StatePhysicsUpdate() { }
}
