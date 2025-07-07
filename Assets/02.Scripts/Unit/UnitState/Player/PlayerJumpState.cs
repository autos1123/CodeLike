using UnityEngine;

public class PlayerJumpState:PlayerBaseState
{
    private bool hasStartedFalling = false;

    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        Debug.Log("Jump 상태 진입");

        float force = player.Condition.GetValue(ConditionType.JumpPower);
        Debug.Log($"Jump force: {force}");

        Vector3 v = player._Rigidbody.velocity;
        Debug.Log($"Current Velocity Before Jump: {v}");

        v.y = 0;
        player._Rigidbody.velocity = v;

        player._Rigidbody.AddForce(Vector3.up * force, ForceMode.Impulse);

        Debug.Log($"Applied Jump force: {Vector3.up * force}");
        player._Animator.SetBool("isJumping", true);

        hasStartedFalling = false; // Jump 진입할 때 초기화!
    }

    public override void StateExit()
    {
        Debug.Log("Jump 상태 종료");
        player._Animator.SetBool("isJumping", false);
    }

    public override void StateUpdate()
    {
        if(player._Rigidbody.velocity.y < -0.1f && !hasStartedFalling)
        {
            //hasStartedFalling = true;
            //stateMachine.ChangeState(stateMachine.FallState); // 낙하 state전환 애니메이션 기능 추가할 예정
        }
    }

    public override void StatePhysicsUpdate()
    {
        Move(player.InputHandler.MoveInput);
    }
}
