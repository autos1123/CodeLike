using System.Linq;
using UnityEngine;

public class PlayerAttack2State:PlayerBaseState
{
    private float comboTimer = 0f;
    private float comboWindowStart = 0f;
    private float comboWindowEnd = 0f;
    private float actualClipLength = 0f;
    private bool canCancel = false;
    private const float MinComboWindow = 0.18f;

    public PlayerAttack2State(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();

        Player.ComboBuffered = false;
        float attackSpeed = Player.Condition.GetTotalCurrentValue(ConditionType.AttackSpeed);
        Player.Animator.SetFloat("AttackSpeed", attackSpeed);

        float baseClipLength = Player.Animator.runtimeAnimatorController.animationClips
            .FirstOrDefault(x => x.name == "Slash Attack").length;
        actualClipLength = baseClipLength / attackSpeed;

        comboWindowStart = actualClipLength * 0.18f;
        comboWindowEnd = Mathf.Max(comboWindowStart + MinComboWindow, actualClipLength * 0.75f);
        comboWindowEnd = Mathf.Min(comboWindowEnd, actualClipLength);

        comboTimer = 0f;
        canCancel = false;

        StartAnimation(Player.AnimationData.Attack2ParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        comboTimer += Time.deltaTime;

        var input = Player.InputHandler;

        if(comboTimer >= comboWindowStart && comboTimer <= comboWindowEnd)
        {
            canCancel = true;
            if(input.AttackPressed)
                Player.ComboBuffered = true;
            if(Player.ComboBuffered)
            {
                Player.ComboBuffered = false;
                stateMachine.ChangeState(stateMachine.Attack3State);
                return;
            }
        }
        else
        {
            canCancel = false;
        }

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

        if(comboTimer > actualClipLength)
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
