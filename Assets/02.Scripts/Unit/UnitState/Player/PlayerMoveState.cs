using UnityEngine;

/// <summary>
/// 플레이어 이동 상태
/// </summary>
public class PlayerMoveState:IUnitState
{
    private PlayerController player;
    private PlayerStateMachine stateMachine;

    public PlayerMoveState(PlayerController player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public void StateEnter() { }
    public void StateExit() { }

    public void StateUpdate()
    {
        var move = player.Input.MoveInput;
        if(move.magnitude < 0.1f)
        {
            stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
        }
    }

    public void StatePhysicsUpdate()
    {
        player.Move(player.Input.MoveInput);
    }
}
