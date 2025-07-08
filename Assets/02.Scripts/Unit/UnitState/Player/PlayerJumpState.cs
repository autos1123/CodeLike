using UnityEngine;

public class PlayerJumpState:PlayerBaseState
{
    private bool hasStartedFalling = false;

    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();

        float force = player.Condition.GetValue(ConditionType.JumpPower);

        Vector3 v = player._Rigidbody.velocity;

        v.y = 0;
        player._Rigidbody.velocity = v;

        player._Rigidbody.AddForce(Vector3.up * force, ForceMode.Impulse);

        StartAnimation(player.AnimationData.JumpParameterHash);

        hasStartedFalling = false; // fall기능 추가 대비용 변수
    }

    public override void StateExit()
    {
        base.StateExit();
        StopAnimation(player.AnimationData.JumpParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        if(player._Rigidbody.velocity.y < -0.1f && !hasStartedFalling)
        {
            //hasStartedFalling = true;
            //stateMachine.ChangeState(stateMachine.FallState); // 낙하 state전환 애니메이션 기능 추가할 예정
        }
    }

    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
        Move(player.InputHandler.MoveInput);
    }
}
