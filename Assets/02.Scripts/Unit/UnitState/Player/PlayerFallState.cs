using UnityEngine;

public class PlayerFallState:IUnitState
{
    private PlayerController player;
    private PlayerStateMachine stateMachine;

    public PlayerFallState(PlayerController player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public void StateEnter()
    {
        Debug.Log("Fall 상태 진입");
        // 추후 활성화 예정
        //player._Animator.SetBool("isFalling", true);
    }

    public void StateExit()
    {
        Debug.Log("Fall 상태 종료");
        // 추후 활성화 예정
        //player._Animator.SetBool("isFalling", false);
    }

    public void StateUpdate()
    {
        //if(player.IsGrounded)
        //{
        //    if(player.Input.MoveInput.magnitude > 0.1f)
        //        stateMachine.ChangeState(new PlayerMoveState(player, stateMachine));
        //    else
        //        stateMachine.ChangeState(new PlayerIdleState(stateMachine));
        //}
    }

    public void StatePhysicsUpdate()
    {
        // player.Move(player.Input.MoveInput);
    }
}
