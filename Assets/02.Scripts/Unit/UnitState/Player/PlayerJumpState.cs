using UnityEngine;

public class PlayerJumpState:PlayerBaseState
{
    private bool hasStartedFalling = false;
    private float jumpStartTime;
    private float jumpGroundGrace = 0.15f; // 점프 후 ground check 무시 시간
    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();

        StartAnimation(Player.AnimationData.JumpParameterHash);

        float force = Player.Condition.GetValue(ConditionType.JumpPower);

        Vector3 v = Player._Rigidbody.velocity;

        v.y = 0;
        Player._Rigidbody.velocity = v;

        Player._Rigidbody.AddForce(Vector3.up * force, ForceMode.Impulse);

        jumpStartTime = Time.time;
        hasStartedFalling = false;
    }


    public override void StateExit()
    {
        base.StateExit();
        StopAnimation(Player.AnimationData.JumpParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        if(Player.InputHandler.AttackPressed)
        {
            // 점프 중 공격 시 상태 전환
            stateMachine.ChangeState(stateMachine.AttackState);
            return;
        }
        Vector2 move = Player.InputHandler.MoveInput;

        if(move.magnitude > 0.1f)
        {
            PlayerLookAt();
        }

        // jumpGrace 기간 동안 착지 체크 안 함
        if(Time.time - jumpStartTime < jumpGroundGrace)
            return;

        if(!Player.IsGrounded)
            return;

        // 착지시 상태 전환
        stateMachine.ChangeState(stateMachine.IdleState);
    }

    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
        Move(Player.InputHandler.MoveInput);
    }
}
