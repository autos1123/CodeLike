using UnityEngine;

public class PlayerJumpState:PlayerBaseState
{
    private bool hasStartedFalling = false;

    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();

        StartAnimation(player.AnimationData.JumpParameterHash);

        float force = player.Condition.GetValue(ConditionType.JumpPower);

        Vector3 v = player._Rigidbody.velocity;

        v.y = 0;
        player._Rigidbody.velocity = v;

        player._Rigidbody.AddForce(Vector3.up * force, ForceMode.Impulse);


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

        if(!player.isGrounded)
            return;

        StopAnimation(player.AnimationData.JumpParameterHash);

        if(player.InputHandler.MoveInput.magnitude > 0.1f)
            stateMachine.ChangeState(stateMachine.MoveState);
        else
            stateMachine.ChangeState(stateMachine.IdleState);

    }


    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
        Move(player.InputHandler.MoveInput);
    }
}
