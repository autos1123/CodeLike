using UnityEngine;

public class PlayerAttack1State:PlayerBaseState
{
    private float comboTimer = 0f;
    private float comboWindow = 0.5f; // 다음 콤보 입력 가능 시간

    public PlayerAttack1State(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();
        StartAnimation(Player.AnimationData.Attack1ParameterHash);
        comboTimer = 0f;

    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        comboTimer += Time.deltaTime;

        Vector2 move = Player.InputHandler.MoveInput;
        if(move.magnitude > 0.1f)
        {
            PlayerLookAt(); // 이동 중 회전
        }

        // 콤보 입력 체크
        if(Player.InputHandler.AttackPressed && comboTimer < comboWindow)
        {
            stateMachine.ChangeState(stateMachine.Attack2State);
            return;
        }

        // 콤보 시간 초과 or 종료
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
