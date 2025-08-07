using System.Linq;
using UnityEngine;

public class PlayerAttack1State:PlayerBaseState
{
    private float comboTimer = 0f;
    private float comboWindowStart = 0f;
    private float comboWindowEnd = 0f;
    private float actualClipLength = 0f;
    private bool canCancel = false;

    private const float MinComboWindow = 0.18f; // 할로우나이트 느낌에 가깝게

    public PlayerAttack1State(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();

        // 콤보 버퍼, 캔슬 모두 초기화
        Player.ComboBuffered = false;

        float attackSpeed = Player.Condition.GetTotalCurrentValue(ConditionType.AttackSpeed);
        Player.Animator.SetFloat("AttackSpeed", attackSpeed);

        float baseClipLength = Player.Animator.runtimeAnimatorController.animationClips
            .FirstOrDefault(x => x.name == "Slap Attack").length;
        actualClipLength = baseClipLength / attackSpeed;

        // 콤보 가능 구간, 캔슬 구간
        comboWindowStart = actualClipLength * 0.20f;
        comboWindowEnd = Mathf.Max(comboWindowStart + MinComboWindow, actualClipLength * 0.7f);
        comboWindowEnd = Mathf.Min(comboWindowEnd, actualClipLength);

        comboTimer = 0f;
        canCancel = false;

        StartAnimation(Player.AnimationData.Attack1ParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        comboTimer += Time.deltaTime;

        var input = Player.InputHandler;

        // 콤보 윈도우 진입
        if(comboTimer >= comboWindowStart && comboTimer <= comboWindowEnd)
        {
            canCancel = true;

            // Attack 입력시 콤보 버퍼 처리 (프레임 단위 입력)
            if(input.AttackPressed)
                Player.ComboBuffered = true;

            // 콤보 버퍼가 들어오면 바로 Attack2State로!
            if(Player.ComboBuffered)
            {
                Player.ComboBuffered = false;
                stateMachine.ChangeState(stateMachine.Attack2State);
                return;
            }
        }
        else
        {
            canCancel = false;
        }

        // 콤보 캔슬 구간(할로우나이트식): 대쉬, 점프, 스킬 즉시 전환
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

    public override void StateExit()
    {
        base.StateExit();
    }

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
