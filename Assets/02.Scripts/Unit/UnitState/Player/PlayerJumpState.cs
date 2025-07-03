using UnityEngine;

public class PlayerJumpState:IUnitState
{
    private PlayerController player;
    private PlayerStateMachine stateMachine;
    private bool hasStartedFalling = false;

    public PlayerJumpState(PlayerController player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public void StateEnter()
    {
        Debug.Log("Jump 상태 진입");

        float force = player.PlayerCondition.GetValue(ConditionType.JumpPower);
        Debug.Log($"Jump force: {force}");

        Vector3 v = player._Rigidbody.velocity;
        Debug.Log($"Current Velocity Before Jump: {v}");

        v.y = 0;
        player._Rigidbody.velocity = v;

        player._Rigidbody.AddForce(Vector3.up * force, ForceMode.Impulse);

        Debug.Log($"Applied Jump force: {Vector3.up * force}");
        player._Animator.SetBool("isJumping", true);
    }


    public void StateExit()
    {
        Debug.Log("Jump 상태 종료");
        player._Animator.SetBool("isJumping", false);
    }

    public void StateUpdate()
    {
        if(player._Rigidbody.velocity.y < -0.1f && !hasStartedFalling)
        {
            hasStartedFalling = true;
            stateMachine.ChangeState(new PlayerFallState(player, stateMachine));
        }
    }

    public void StatePhysicsUpdate()
    {
        player.Move(player.Input.MoveInput);
    }
}
