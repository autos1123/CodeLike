using UnityEngine;

public class PlayerSkillState:PlayerBaseState
{
    private Skillinput usingSkill = Skillinput.None;
    private float skillDuration = 0.5f; // 예시용
    private float timer = 0f;

    public PlayerSkillState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public void SetSkill(Skillinput skill)
    {
        usingSkill = skill;
    }

    public override void StateEnter()
    {
        base.StateEnter();
        bool success = Player.ActiveItemController.UseItem(usingSkill);
        if(!success)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }

        Debug.LogWarning("PlayerSkillState Entered with skill: " + usingSkill);
        StartAnimation(Player.AnimationData.SkillParameterHash);
        Player.ActiveItemController.UseItem(usingSkill);
        timer = 0f;
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        timer += Time.deltaTime;
        Vector2 move = Player.InputHandler.MoveInput;
        if(move.magnitude > 0.1f)
        {
            PlayerLookAt();
        }
        if(timer > skillDuration)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    public override void StateExit()
    {
        base.StateExit();
        StopAnimation(Player.AnimationData.SkillParameterHash);
        // 필요시 상태 초기화, 입력 해제 등
    }

    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
        Move(Player.InputHandler.MoveInput);
    }
}
