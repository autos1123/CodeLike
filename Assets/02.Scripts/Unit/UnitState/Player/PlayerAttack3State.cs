using UnityEngine;

public class PlayerAttack3State:PlayerBaseState
{
    private float comboTimer = 0f;
    private float comboWindow = 0.5f;

    public PlayerAttack3State(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();
        Debug.LogWarning("PlayerAttack3State Entered");
        StartAnimation(Player.AnimationData.Attack3ParameterHash);
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

        // 마지막 콤보니까 추가 입력 없어도 Idle로 복귀
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
