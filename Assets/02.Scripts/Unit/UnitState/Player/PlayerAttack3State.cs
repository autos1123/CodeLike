using System.Linq;
using UnityEngine;

public class PlayerAttack3State:PlayerBaseState
{
    private float timer = 0f;
    private float actualClipLength = 0f;
    private bool canCancel = false;
    private const float cancelStart = 0.15f;

    public PlayerAttack3State(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();

        float attackSpeed = Player.Condition.GetTotalCurrentValue(ConditionType.AttackSpeed);
        Player.Animator.SetFloat("AttackSpeed", attackSpeed);

        float baseClipLength = Player.Animator.runtimeAnimatorController.animationClips
            .FirstOrDefault(x => x.name == "Clap Attack").length;
        actualClipLength = baseClipLength / attackSpeed;

        timer = 0f;
        canCancel = false;

        StartAnimation(Player.AnimationData.Attack3ParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        timer += Time.deltaTime;
        var input = Player.InputHandler;

        if(timer >= actualClipLength * cancelStart)
            canCancel = true;

        if(canCancel)
        {
            if(input.DashPressed)
            {
                stateMachine.ChangeState(stateMachine.DashState);
                return;
            }
            if(input.JumpPressed && Player.IsGrounded)
            {
                stateMachine.ChangeState(stateMachine.JumpState);
                return;
            }
            if(input.SkillXPressed)
            {
                stateMachine.SkillState.SetSkill(Skillinput.X);
                stateMachine.ChangeState(stateMachine.SkillState);
                return;
            }
            if(input.SkillCPressed)
            {
                stateMachine.SkillState.SetSkill(Skillinput.C);
                stateMachine.ChangeState(stateMachine.SkillState);
                return;
            }
        }

        if(timer > actualClipLength)
            stateMachine.ChangeState(stateMachine.IdleState);
    }

    public override void StateExit() { base.StateExit(); }
    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
        if(canCancel)
        {

            Move(Player.InputHandler.MoveInput);
        }
        else
        {
            // 캔슬 불가 구간(이동은 되지만 속도 제한, 혹은 아주 느리게만 이동)
            Move(Player.InputHandler.MoveInput * 0.4f); // or 0.2f, 취향에 따라
        }
    }
}
