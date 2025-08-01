using UnityEngine;

public class PlayerAttack2State:PlayerBaseState
{
    private float comboTimer = 0f;
    private float comboWindow = 0.5f;

    public PlayerAttack2State(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();
        StartAnimation(Player.AnimationData.Attack2ParameterHash);
        comboTimer = 0f;

    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        comboTimer += Time.deltaTime;
        Vector2 move = Player.InputHandler.MoveInput;
        if(move.magnitude > 0.1f)
        {
            PlayerLookAt();
        }

        if(Player.InputHandler.AttackPressed && comboTimer < comboWindow)
        {
            stateMachine.ChangeState(stateMachine.Attack3State);
            return;
        }

        if(comboTimer > comboWindow)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    public override void StateExit()
    {
        base.StateExit();
    }
    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
        Move(Player.InputHandler.MoveInput);
    }
}
