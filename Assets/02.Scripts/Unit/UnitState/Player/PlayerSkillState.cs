using UnityEngine;

public class PlayerSkillState:PlayerBaseState
{
    private Skillinput usingSkill = Skillinput.None;

    public PlayerSkillState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public void SetSkill(Skillinput skill)
    {
        usingSkill = skill;
    }

    public void UseSkill()
    {
        Player.ActiveItemController.UseItem(usingSkill);
    }

    public override void StateEnter()
    {
        base.StateEnter();
        if(!Player.ActiveItemController.CanUseSkill(usingSkill))
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
        Player.OnSkillInput += UseSkill;
        Debug.LogWarning("PlayerSkillState Entered with skill: " + usingSkill);
        StartAnimation(Player.AnimationData.SkillParameterHash);

    }


    public override void StateUpdate()
    {
        base.StateUpdate();
        Vector2 move = Player.InputHandler.MoveInput;
        if(move.magnitude > 0.1f)
        {
            PlayerLookAt();
        }
        if(Player._Animator.GetCurrentAnimatorStateInfo(0).IsName("Cast") &&
            Player._Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    public override void StateExit()
    {
        base.StateExit();
        Player.OnSkillInput -= UseSkill;
        StopAnimation(Player.AnimationData.SkillParameterHash);
    }

    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
        Move(Player.InputHandler.MoveInput);
    }
}
