using UnityEngine;

public class PlayerJumpState:PlayerBaseState
{
    private bool hasStartedFalling = false;
    private float jumpStartTime;
    private float jumpGroundGrace = 0.05f; // 점프 후 ground check 무시 시간
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
        Vector2 move = Player.InputHandler.MoveInput;
        if(move.magnitude > 0.1f)
        {
            PlayerLookAt();
        }

        // jumpGroundGrace 동안 낙하/착지 체크 안함
        if(Time.time - jumpStartTime < jumpGroundGrace)
            return;

        // 하강 시작 감지
        if(Player._Rigidbody.velocity.y <= 0f)
        {
            Debug.Log(">> FallState로 전환 시도!");
            Debug.Log($"stateMachine.FallState is null? {stateMachine.FallState == null}");
            stateMachine.ChangeState(stateMachine.FallState);
            return;
        }
    }


    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
        Move(Player.InputHandler.MoveInput);
    }
}
